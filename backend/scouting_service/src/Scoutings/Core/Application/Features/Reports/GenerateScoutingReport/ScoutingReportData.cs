namespace scouting_service.src.Scoutings.Core.Application.Features.Reports.GenerateScoutingReport;

public class ScoutingReportData
{
    public int PlayerId { get; set; }
    public string PlayerFullName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string NationalityName { get; set; } = string.Empty;
    public int? LatestHeight { get; set; }
    public int? LatestWeight { get; set; }
    public DateTime? LatestJumpDate { get; set; }
    public int? LatestVerticalJump { get; set; }
    public int TotalSessionsAnalyzed { get; set; }
    public decimal TotalScoutingScore { get; set; }
    public decimal NormalizedScore { get; set; }
    public DateTime ReportDate { get; set; }
}