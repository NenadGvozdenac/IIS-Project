namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSeasonMetricAverages;

public class GetPlayerSeasonMetricAveragesResponse
{
    public int MetricId { get; set; }
    public string MetricName { get; set; } = null!;
    public int MetricWeight { get; set; }
    public decimal AverageValue { get; set; }
    public int SessionCount { get; set; }
}