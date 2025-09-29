namespace scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.UpdateSessionMetric;

public class UpdateSessionMetricResponse
{
    public string Value { get; set; } = null!;
    public int IdSession { get; set; }
    public int IdMetrics { get; set; }
}