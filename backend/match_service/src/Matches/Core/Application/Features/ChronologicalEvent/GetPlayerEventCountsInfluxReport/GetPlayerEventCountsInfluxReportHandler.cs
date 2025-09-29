using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerEventCountsInfluxReport
{
    public class GetPlayerEventCountsInfluxReportHandler : IRequestHandler<GetPlayerEventCountsInfluxReportQuery, GetPlayerEventCountsInfluxReportResponse>
    {
        private readonly IChronologicalEventInfluxRepository _influxRepository;
        private readonly ILogger<GetPlayerEventCountsInfluxReportHandler> _logger;

        public GetPlayerEventCountsInfluxReportHandler(
            IChronologicalEventInfluxRepository influxRepository,
            ILogger<GetPlayerEventCountsInfluxReportHandler> logger)
        {
            _influxRepository = influxRepository;
            _logger = logger;
        }

        public async Task<GetPlayerEventCountsInfluxReportResponse> Handle(GetPlayerEventCountsInfluxReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.PlayerId))
                {
                    _logger.LogWarning("GetPlayerEventCountsInfluxReport: PlayerId is required");
                    return GetPlayerEventCountsInfluxReportResponse.CreateError("PlayerId is required");
                }

                if (request.StartTime >= request.EndTime)
                {
                    _logger.LogWarning("GetPlayerEventCountsInfluxReport: StartTime must be before EndTime");
                    return GetPlayerEventCountsInfluxReportResponse.CreateError("StartTime must be before EndTime");
                }

                var playerEventCounts = await _influxRepository.GetPlayerEventCountsInfluxReportAsync(
                    request.PlayerId, request.StartTime, request.EndTime);

                _logger.LogInformation("Successfully retrieved event counts for player {PlayerId} from {StartTime} to {EndTime}: {Count} matches", 
                    request.PlayerId, request.StartTime, request.EndTime, playerEventCounts.Count());

                return GetPlayerEventCountsInfluxReportResponse.CreateSuccess(playerEventCounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event counts for player {PlayerId}", request.PlayerId);
                return GetPlayerEventCountsInfluxReportResponse.CreateError($"Failed to retrieve player event counts: {ex.Message}");
            }
        }
    }
}