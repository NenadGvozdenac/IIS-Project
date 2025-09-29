using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities.Relational;

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
}
