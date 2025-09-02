using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class PersonalEvent
{
    public int IdEvent { get; set; }

    public DateOnly? CreationTime { get; set; }

    public string? Notes { get; set; }

    public string? Role { get; set; }

    public string? Type { get; set; }

    public int IdTeam { get; set; }

    public int IdPlayer { get; set; }

    public int IdMatch { get; set; }

    public virtual TeamMember Id { get; set; } = null!;

    public virtual MatchTracking IdMatchNavigation { get; set; } = null!;
}
