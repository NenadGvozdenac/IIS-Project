namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.GetAllMetrics;

public class GetAllMetricsResponse
{
    public int IdMetrics { get; set; }
    public string? Name { get; set; }
    public int? IsPermanent { get; set; }
    public int? MetricWeight { get; set; }
    public int IdUser { get; set; }
    public string? UserName { get; set; }
    public int IdMetricType { get; set; }
    public string? MetricTypeName { get; set; }
}
