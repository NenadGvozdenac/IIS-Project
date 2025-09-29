using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetSeasonPlayerAverages
{
    public class GetSeasonPlayerAveragesHandler : IRequestHandler<GetSeasonPlayerAveragesQuery, Result<GetSeasonPlayerAveragesResponse>>
    {
        private readonly IChronologicalEventInfluxRepository _influxRepository;
        private readonly ILogger<GetSeasonPlayerAveragesHandler> _logger;

        public GetSeasonPlayerAveragesHandler(
            IChronologicalEventInfluxRepository influxRepository,
            ILogger<GetSeasonPlayerAveragesHandler> logger)
        {
            _influxRepository = influxRepository;
            _logger = logger;
        }

        public async Task<Result<GetSeasonPlayerAveragesResponse>> Handle(GetSeasonPlayerAveragesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Executing season player averages query for period {StartDate} to {EndDate}, TeamId: {TeamId}, MinMatches: {MinMatches}", 
                    request.StartDate, request.EndDate, request.TeamId ?? "All", request.MinMatches);

                var rawResults = await _influxRepository.GetSeasonPlayerAveragesAsync(
                    request.StartDate, 
                    request.EndDate, 
                    request.TeamId, 
                    request.MinMatches);

                _logger.LogInformation("Repository returned {Count} raw results", rawResults?.Count() ?? 0);

                if (rawResults == null || !rawResults.Any())
                {
                    _logger.LogWarning("No player data found for the specified period {StartDate} to {EndDate}, TeamId: {TeamId}", 
                        request.StartDate, request.EndDate, request.TeamId ?? "All");
                    
                    return Result<GetSeasonPlayerAveragesResponse>.Success(new GetSeasonPlayerAveragesResponse
                    {
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        TeamId = request.TeamId,
                        PlayerAverages = new List<PlayerSeasonAverage>(),
                        Description = "No player data found for the specified period"
                    });
                }

                var playerAverages = new List<PlayerSeasonAverage>();

                foreach (var result in rawResults)
                {
                    var playerId = result.PlayerId?.ToString() ?? "";
                    var matchesPlayed = Convert.ToInt32(result.MatchesPlayed ?? 0);
                    var avgPoints = Convert.ToDouble(result.AvgPoints ?? 0);
                    
                    _logger.LogDebug("Processing player result - ID: {PlayerId}, Matches: {Matches}, AvgPoints: {Points}", 
                        (object)playerId, (object)matchesPlayed, (object)avgPoints);

                    playerAverages.Add(new PlayerSeasonAverage
                    {
                        PlayerId = result.PlayerId?.ToString() ?? "",
                        MatchesPlayed = Convert.ToInt32(result.MatchesPlayed ?? 0),
                        AvgPoints = Convert.ToDouble(result.AvgPoints ?? 0),
                        AvgAssists = Convert.ToDouble(result.AvgAssists ?? 0),
                        AvgFouls = Convert.ToDouble(result.AvgFouls ?? 0),
                        PointsStdDev = Convert.ToDouble(result.PointsStdDev ?? 0),
                        TotalPoints = Convert.ToDouble(result.TotalPoints ?? 0),
                        TotalAssists = Convert.ToDouble(result.TotalAssists ?? 0),
                        TotalFouls = Convert.ToDouble(result.TotalFouls ?? 0)
                    });
                }

                var response = new GetSeasonPlayerAveragesResponse
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TeamId = request.TeamId,
                    PlayerAverages = playerAverages.OrderByDescending(p => p.AvgPoints).ToList(),
                    Description = $"Season averages analysis across {playerAverages.Count} players with comprehensive aggregation, filtering and sorting"
                };

                _logger.LogInformation("Successfully retrieved season averages for {PlayerCount} players", playerAverages.Count);

                return Result<GetSeasonPlayerAveragesResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving season player averages for period {StartDate} to {EndDate}", 
                    request.StartDate, request.EndDate);
                return Result<GetSeasonPlayerAveragesResponse>.Failure($"Error retrieving season player averages: {ex.Message}");
            }
        }
    }
}