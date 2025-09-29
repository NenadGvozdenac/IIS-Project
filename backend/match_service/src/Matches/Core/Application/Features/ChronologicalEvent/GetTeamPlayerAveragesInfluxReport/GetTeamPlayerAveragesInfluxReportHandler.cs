using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetTeamPlayerAveragesInfluxReport
{
    public class GetTeamPlayerAveragesInfluxReportHandler : IRequestHandler<GetTeamPlayerAveragesInfluxReportQuery, GetTeamPlayerAveragesInfluxReportResponse>
    {
        private readonly IChronologicalEventInfluxRepository _influxRepository;
        private readonly ILogger<GetTeamPlayerAveragesInfluxReportHandler> _logger;

        public GetTeamPlayerAveragesInfluxReportHandler(
            IChronologicalEventInfluxRepository influxRepository,
            ILogger<GetTeamPlayerAveragesInfluxReportHandler> logger)
        {
            _influxRepository = influxRepository;
            _logger = logger;
        }

        public async Task<GetTeamPlayerAveragesInfluxReportResponse> Handle(GetTeamPlayerAveragesInfluxReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.StartTime >= request.EndTime)
                {
                    _logger.LogWarning("GetTeamPlayerAveragesInfluxReport: StartTime must be before EndTime");
                    return GetTeamPlayerAveragesInfluxReportResponse.CreateError("StartTime must be before EndTime");
                }

                var teamPlayerAverages = await _influxRepository.GetTeamPlayerAveragesInfluxReportAsync(
                    request.StartTime, request.EndTime, request.TeamId);

                _logger.LogInformation("Successfully retrieved team player averages for team {TeamId} from {StartTime} to {EndTime}: {Count} players", 
                    request.TeamId, request.StartTime, request.EndTime, teamPlayerAverages.Count());

                return GetTeamPlayerAveragesInfluxReportResponse.CreateSuccess(teamPlayerAverages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving team player averages for team {TeamId}", request.TeamId);
                return GetTeamPlayerAveragesInfluxReportResponse.CreateError($"Failed to retrieve team player averages: {ex.Message}");
            }
        }
    }
}