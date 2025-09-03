using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class GeneralEvent
{
    public int IdEvent { get; set; }

    public DateTime CreationTime { get; set; }

    public string? Notes { get; set; }

    public string? Role { get; set; }

    public string? Type { get; set; }

    public int IdMatch { get; set; }

    public virtual MatchTracking IdMatchNavigation { get; set; } = null!;
}
