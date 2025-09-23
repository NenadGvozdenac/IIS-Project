namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerById;

public class GetPlayerByIdResponse
{
    public int IdPlayer { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public DateOnly? Birthday { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int IdNationality { get; set; }
    public string? NationalityName { get; set; }
    public int IdPosition { get; set; }
    public string? PositionName { get; set; }
    public PhysicalMetricResponse? LatestPhysicalMetric { get; set; }
}

public class PhysicalMetricResponse
{
    public int IdPhysicalMetrics { get; set; }
    public int? VerticalJump { get; set; }
    public int? FatPercentage { get; set; }
    public int? BenchPressWeight { get; set; }
    public int? SquatWeight { get; set; }
    public decimal? SprintSpeed { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int? Wingspan { get; set; }
    public DateOnly? DateOfMeasurement { get; set; }
    public int IdPlayer { get; set; }
}
