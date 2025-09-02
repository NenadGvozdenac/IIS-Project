using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class Season
{
    public int IdSeason { get; set; }

    public DateOnly StartedAt { get; set; }

    public DateOnly? EndedAt { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}
