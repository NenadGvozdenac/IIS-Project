namespace scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.GetAllSessionMetrics;

public class GetAllSessionMetricsResponse
{
    public List<SessionMetricDto> SessionMetrics { get; set; } = new();
}

public class SessionMetricDto
{
    public string? Value { get; set; }
    public int IdSession { get; set; }
    public int IdMetrics { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public string MetricTypeName { get; set; } = string.Empty;
}
