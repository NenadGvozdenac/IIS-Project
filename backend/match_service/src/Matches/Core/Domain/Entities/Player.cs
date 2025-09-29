using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class Player
{
    public int IdPlayer { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public DateOnly? Birthday { get; set; }

    public int? Weight { get; set; }

    public int? Height { get; set; }

    public int IdNationality { get; set; }

    public int IdPosition { get; set; }

    public virtual Nationality IdNationalityNavigation { get; set; } = null!;

    public virtual Position IdPositionNavigation { get; set; } = null!;

    public virtual ICollection<PhysicalMetric> PhysicalMetrics { get; set; } = new List<PhysicalMetric>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
