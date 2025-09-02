using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class TeamMemberMatch
{
    public bool StartingLineup { get; set; }

    public bool InGame { get; set; }

    public int IdTeam { get; set; }

    public int IdPlayer { get; set; }

    public int IdMatch { get; set; }

    public virtual TeamMember Id { get; set; } = null!;

    public virtual MatchTracking IdMatchNavigation { get; set; } = null!;
}
