using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerPerformanceComparison
{
    public class GetPlayerPerformanceComparisonHandler : IRequestHandler<GetPlayerPerformanceComparisonQuery, Result<GetPlayerPerformanceComparisonResponse>>
    {
        private readonly IChronologicalEventInfluxRepository _influxRepository;
        private readonly ILogger<GetPlayerPerformanceComparisonHandler> _logger;

        public GetPlayerPerformanceComparisonHandler(
            IChronologicalEventInfluxRepository influxRepository,
            ILogger<GetPlayerPerformanceComparisonHandler> logger)
        {
            _influxRepository = influxRepository;
            _logger = logger;
        }

        public async Task<Result<GetPlayerPerformanceComparisonResponse>> Handle(GetPlayerPerformanceComparisonQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MatchId))
                {
                    return Result<GetPlayerPerformanceComparisonResponse>.Failure("Match ID is required");
                }

                _logger.LogInformation("Executing player performance comparison query for match {MatchId}", request.MatchId);

                var rawResults = await _influxRepository.GetPlayerPerformanceComparisonAsync(request.MatchId);

                if (!rawResults.Any())
                {
                    _logger.LogWarning("No player events found for match {MatchId} in InfluxDB", request.MatchId);
                    return Result<GetPlayerPerformanceComparisonResponse>.Success(new GetPlayerPerformanceComparisonResponse
                    {
                        MatchId = request.MatchId,
                        PlayerRankings = new List<PlayerPerformance>()
                    });
                }

                // Group results by player
                var playerGroups = rawResults.GroupBy(r => r.PlayerId?.ToString() ?? "").ToList();
                var playerPerformances = new List<PlayerPerformance>();

                foreach (var playerGroup in playerGroups)
                {
                    var playerId = playerGroup.Key;
                    if (string.IsNullOrEmpty(playerId)) continue;

                    var eventTypeBreakdown = new Dictionary<string, int>();
                    var totalEvents = 0;
                    var totalPoints = 0.0;

                    foreach (var result in playerGroup)
                    {
                        var eventType = result.EventType?.ToString() ?? "";
                        var events = Convert.ToInt32(result.TotalEvents ?? result.Count ?? 0);
                        
                        if (!string.IsNullOrEmpty(eventType))
                        {
                            if (eventTypeBreakdown.ContainsKey(eventType))
                                eventTypeBreakdown[eventType] += events;
                            else
                                eventTypeBreakdown[eventType] = events;
                        }
                        totalEvents += events;

                        // Calculate points based on event type
                        totalPoints += CalculatePoints(eventType, events);
                    }

                    var performanceScore = CalculatePerformanceScore(eventTypeBreakdown);

                    playerPerformances.Add(new PlayerPerformance
                    {
                        PlayerId = playerId,
                        TotalEvents = totalEvents,
                        TotalPoints = totalPoints,
                        PerformanceScore = performanceScore,
                        EventTypeBreakdown = eventTypeBreakdown
                    });
                }

                // Rank players by performance score
                var rankedPlayers = playerPerformances
                    .OrderByDescending(p => p.PerformanceScore)
                    .Select((player, index) => 
                    {
                        player.Rank = index + 1;
                        return player;
                    })
                    .ToList();

                var response = new GetPlayerPerformanceComparisonResponse
                {
                    MatchId = request.MatchId,
                    PlayerRankings = rankedPlayers
                };

                _logger.LogInformation("Successfully ranked {Count} players for match {MatchId}", 
                    rankedPlayers.Count, request.MatchId);

                return Result<GetPlayerPerformanceComparisonResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player performance comparison for match {MatchId}", request.MatchId);
                return Result<GetPlayerPerformanceComparisonResponse>.Failure($"Error retrieving player performance comparison: {ex.Message}");
            }
        }

        private static double CalculatePoints(string eventType, int count)
        {
            return eventType switch
            {
                "+2p" => count * 2.0,
                "+3p" => count * 3.0,
                "+ft" => count * 1.0,
                _ => 0.0
            };
        }

        private static double CalculatePerformanceScore(Dictionary<string, int> eventBreakdown)
        {
            var score = 0.0;
            
            // Positive scoring events
            score += eventBreakdown.GetValueOrDefault("+2p", 0) * 2.0;
            score += eventBreakdown.GetValueOrDefault("+3p", 0) * 3.0;
            score += eventBreakdown.GetValueOrDefault("+ft", 0) * 1.0;
            score += eventBreakdown.GetValueOrDefault("assist", 0) * 1.0;
            score += eventBreakdown.GetValueOrDefault("reb of", 0) * 1.0;
            score += eventBreakdown.GetValueOrDefault("reb def", 0) * 1.0;
            score += eventBreakdown.GetValueOrDefault("steal", 0) * 1.0;
            score += eventBreakdown.GetValueOrDefault("block", 0) * 1.0;
            
            // Negative scoring events
            score -= eventBreakdown.GetValueOrDefault("foul", 0) * 1.0;
            score -= eventBreakdown.GetValueOrDefault("2p", 0) * 1.0;
            score -= eventBreakdown.GetValueOrDefault("3p", 0) * 1.0;
            score -= eventBreakdown.GetValueOrDefault("ft", 0) * 1.0;

            return Math.Max(0, score); // Ensure non-negative score
        }
    }
}