using MediatR;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetTeamPlayerAveragesInfluxReport
{
    public class GetTeamPlayerAveragesInfluxReportQuery : IRequest<GetTeamPlayerAveragesInfluxReportResponse>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? TeamId { get; set; } = "1";

        public GetTeamPlayerAveragesInfluxReportQuery(DateTime startTime, DateTime endTime, string? teamId = "1")
        {
            StartTime = startTime;
            EndTime = endTime;
            TeamId = teamId;
        }
    }
}