using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class PhysicalMetric
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

    public virtual Player IdPlayerNavigation { get; set; } = null!;
}
