using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Team
{
    public int IdTeam { get; set; }

    public string Name { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Hall { get; set; } = null!;

    public DateOnly? FoundedDate { get; set; }

    public string? Coach { get; set; }

    public string? PlayingStyle { get; set; }

    public string? KeyStrengths { get; set; }

    public string? KeyWeaknesses { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
