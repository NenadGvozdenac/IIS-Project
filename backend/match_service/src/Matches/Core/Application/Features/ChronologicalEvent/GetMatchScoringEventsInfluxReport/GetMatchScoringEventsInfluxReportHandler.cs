using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchScoringEventsInfluxReport
{
    public class GetMatchScoringEventsInfluxReportHandler : IRequestHandler<GetMatchScoringEventsInfluxReportQuery, GetMatchScoringEventsInfluxReportResponse>
    {
        private readonly IChronologicalEventInfluxRepository _influxRepository;
        private readonly ILogger<GetMatchScoringEventsInfluxReportHandler> _logger;

        public GetMatchScoringEventsInfluxReportHandler(
            IChronologicalEventInfluxRepository influxRepository,
            ILogger<GetMatchScoringEventsInfluxReportHandler> logger)
        {
            _influxRepository = influxRepository;
            _logger = logger;
        }

        public async Task<GetMatchScoringEventsInfluxReportResponse> Handle(GetMatchScoringEventsInfluxReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MatchId))
                {
                    _logger.LogWarning("GetMatchScoringEventsInfluxReport: MatchId is required");
                    return GetMatchScoringEventsInfluxReportResponse.CreateError("MatchId is required");
                }

                var scoringEvents = await _influxRepository.GetMatchScoringEventsInfluxReportAsync(request.MatchId);

                _logger.LogInformation("Successfully retrieved {Count} scoring events for match {MatchId}", 
                    scoringEvents.Count(), request.MatchId);

                return GetMatchScoringEventsInfluxReportResponse.CreateSuccess(scoringEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scoring events for match {MatchId}", request.MatchId);
                return GetMatchScoringEventsInfluxReportResponse.CreateError($"Failed to retrieve scoring events: {ex.Message}");
            }
        }
    }
}