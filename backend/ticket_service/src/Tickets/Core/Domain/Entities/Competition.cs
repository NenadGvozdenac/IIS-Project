using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class Competition
{
    public int IdCompetition { get; set; }

    public string? Name { get; set; }

    public DateOnly? StartedAt { get; set; }

    public DateOnly? EndedAt { get; set; }

    public int? NumberOfMatches { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}
