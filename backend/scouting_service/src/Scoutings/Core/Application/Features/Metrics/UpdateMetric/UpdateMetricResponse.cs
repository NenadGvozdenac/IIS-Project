namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.UpdateMetric;

public class UpdateMetricResponse
{
    public int IdMetrics { get; set; }
    public string Name { get; set; } = null!;
    public int IsPermanent { get; set; }
    public int MetricWeight { get; set; }
    public int IdUser { get; set; }
    public int IdMetricType { get; set; }
}