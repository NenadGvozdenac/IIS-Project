namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerEventCountsInfluxReport
{
    public class GetPlayerEventCountsInfluxReportResponse
    {
        public IEnumerable<dynamic> PlayerEventCounts { get; set; } = new List<dynamic>();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static GetPlayerEventCountsInfluxReportResponse CreateSuccess(IEnumerable<dynamic> playerEventCounts)
        {
            return new GetPlayerEventCountsInfluxReportResponse
            {
                PlayerEventCounts = playerEventCounts,
                Success = true
            };
        }

        public static GetPlayerEventCountsInfluxReportResponse CreateError(string errorMessage)
        {
            return new GetPlayerEventCountsInfluxReportResponse
            {
                PlayerEventCounts = new List<dynamic>(),
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}