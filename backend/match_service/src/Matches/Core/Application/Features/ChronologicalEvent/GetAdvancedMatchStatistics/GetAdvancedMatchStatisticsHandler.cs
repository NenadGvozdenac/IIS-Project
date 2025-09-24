using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetAdvancedMatchStatistics
{
    public class GetAdvancedMatchStatisticsHandler : IRequestHandler<GetAdvancedMatchStatisticsQuery, Result<GetAdvancedMatchStatisticsResponse>>
    {
        private readonly IChronologicalEventInfluxRepository _influxRepository;
        private readonly ILogger<GetAdvancedMatchStatisticsHandler> _logger;

        public GetAdvancedMatchStatisticsHandler(
            IChronologicalEventInfluxRepository influxRepository,
            ILogger<GetAdvancedMatchStatisticsHandler> logger)
        {
            _influxRepository = influxRepository;
            _logger = logger;
        }

        public async Task<Result<GetAdvancedMatchStatisticsResponse>> Handle(GetAdvancedMatchStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MatchId))
                {
                    return Result<GetAdvancedMatchStatisticsResponse>.Failure("Match ID is required");
                }

                _logger.LogInformation("Executing advanced match statistics query for match {MatchId}", request.MatchId);

                var rawResults = await _influxRepository.GetAdvancedMatchStatisticsAsync(request.MatchId);

                if (!rawResults.Any())
                {
                    _logger.LogWarning("No events found for match {MatchId} in InfluxDB", request.MatchId);
                    return Result<GetAdvancedMatchStatisticsResponse>.Success(new GetAdvancedMatchStatisticsResponse
                    {
                        MatchId = request.MatchId,
                        PeriodStatistics = new List<PeriodStatistics>()
                    });
                }

                var periodStatistics = new List<PeriodStatistics>();

                foreach (var result in rawResults)
                {
                    // Convert dynamic results to strongly typed objects
                    var period = result.Period?.ToString() ?? "";
                    var eventType = result.EventType?.ToString() ?? "";
                    var count = Convert.ToInt32(result.Count ?? 0);
                    var timestamp = result.Timestamp as DateTime?;

                    // Calculate efficiency rating (example calculation)
                    var efficiencyRating = CalculateEfficiencyRating(eventType, count);

                    periodStatistics.Add(new PeriodStatistics
                    {
                        Period = period,
                        EventType = eventType,
                        Count = count,
                        Timestamp = timestamp,
                        EfficiencyRating = efficiencyRating,
                        PlayerId = "" // Will be populated if available in repository result
                    });
                }

                var response = new GetAdvancedMatchStatisticsResponse
                {
                    MatchId = request.MatchId,
                    PeriodStatistics = periodStatistics.OrderBy(p => p.Period).ThenBy(p => p.EventType).ToList()
                };

                _logger.LogInformation("Successfully retrieved {Count} period statistics for match {MatchId}", 
                    periodStatistics.Count, request.MatchId);

                return Result<GetAdvancedMatchStatisticsResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving advanced match statistics for match {MatchId}", request.MatchId);
                return Result<GetAdvancedMatchStatisticsResponse>.Failure($"Error retrieving advanced match statistics: {ex.Message}");
            }
        }

        private static double CalculateEfficiencyRating(string eventType, int count)
        {
            // Simple efficiency calculation based on event type
            return eventType switch
            {
                "+2p" => count * 2.0,
                "+3p" => count * 3.0,
                "+ft" => count * 1.0,
                "assist" => count * 1.5,
                "foul" => count * -0.5,
                _ => count * 1.0
            };
        }
    }
}