namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchScoringEventsInfluxReport
{
    public class GetMatchScoringEventsInfluxReportResponse
    {
        public IEnumerable<dynamic> ScoringEvents { get; set; } = new List<dynamic>();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static GetMatchScoringEventsInfluxReportResponse CreateSuccess(IEnumerable<dynamic> scoringEvents)
        {
            return new GetMatchScoringEventsInfluxReportResponse
            {
                ScoringEvents = scoringEvents,
                Success = true
            };
        }

        public static GetMatchScoringEventsInfluxReportResponse CreateError(string errorMessage)
        {
            return new GetMatchScoringEventsInfluxReportResponse
            {
                ScoringEvents = new List<dynamic>(),
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}