namespace scouting_service.src.Scoutings.Core.Application.Features.PhysicalMetrics.CreatePhysicalMetric;

public class CreatePhysicalMetricResponse
{
    public int IdPhysicalMetrics { get; set; }
    public int? VerticalJump { get; set; }
    public int? FatPercentage { get; set; }
    public int? BenchPressWeight { get; set; }
    public int? SquatWeight { get; set; }
    public int? SprintSpeed { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int? Wingspan { get; set; }
    public DateOnly? DateOfMeasurement { get; set; }
    public int IdPlayer { get; set; }
}
