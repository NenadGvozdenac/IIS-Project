using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class TeamEvent
{
    public int IdEvent { get; set; }

    public DateTime CreationTime { get; set; }

    public string? Notes { get; set; }

    public string? Type { get; set; }

    public string? Period { get; set; }

    public int? PeriodTime { get; set; }

    public int IdTeam { get; set; }

    public int IdMatch { get; set; }

    public virtual MatchTracking IdMatchNavigation { get; set; } = null!;

    public virtual Team IdTeamNavigation { get; set; } = null!;
}
