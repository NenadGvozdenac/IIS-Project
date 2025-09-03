using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class AutomaticRecommendation
{
    public string? Priority { get; set; }

    public string? Status { get; set; }

    public DateTime CreationTime { get; set; }

    public int IdMatch { get; set; }

    public virtual MatchTracking IdMatchNavigation { get; set; } = null!;
}
