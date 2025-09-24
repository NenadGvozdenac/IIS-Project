namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetTeamPlayerAveragesInfluxReport
{
    public class GetTeamPlayerAveragesInfluxReportResponse
    {
        public IEnumerable<dynamic> TeamPlayerAverages { get; set; } = new List<dynamic>();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static GetTeamPlayerAveragesInfluxReportResponse CreateSuccess(IEnumerable<dynamic> teamPlayerAverages)
        {
            return new GetTeamPlayerAveragesInfluxReportResponse
            {
                TeamPlayerAverages = teamPlayerAverages,
                Success = true
            };
        }

        public static GetTeamPlayerAveragesInfluxReportResponse CreateError(string errorMessage)
        {
            return new GetTeamPlayerAveragesInfluxReportResponse
            {
                TeamPlayerAverages = new List<dynamic>(),
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}