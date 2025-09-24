using MediatR;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerEventCountsInfluxReport
{
    public class GetPlayerEventCountsInfluxReportQuery : IRequest<GetPlayerEventCountsInfluxReportResponse>
    {
        public string PlayerId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public GetPlayerEventCountsInfluxReportQuery(string playerId, DateTime startTime, DateTime endTime)
        {
            PlayerId = playerId;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}