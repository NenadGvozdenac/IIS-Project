using MediatR;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchScoringEventsInfluxReport
{
    public class GetMatchScoringEventsInfluxReportQuery : IRequest<GetMatchScoringEventsInfluxReportResponse>
    {
        public string MatchId { get; set; } = string.Empty;

        public GetMatchScoringEventsInfluxReportQuery(string matchId)
        {
            MatchId = matchId;
        }
    }
}