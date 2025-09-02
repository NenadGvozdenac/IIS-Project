using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class MatchTracking
{
    public DateOnly? StartTime { get; set; }

    public DateOnly? EndTime { get; set; }

    public string? TrackingStatus { get; set; }

    public int? PeriodDuration { get; set; }

    public string? CurrentPeriod { get; set; }

    public string? PeriodStatus { get; set; }

    public DateOnly? PeriodStartTime { get; set; }

    public int? ElapsedPeriodTime { get; set; }

    public DateOnly? LastPauseStartTime { get; set; }

    public int? TotalPauseTimeInPeriod { get; set; }

    public DateOnly? LastUpdateTime { get; set; }

    public int? OurPoints { get; set; }

    public int? OpponentPoints { get; set; }

    public int? IdUser { get; set; }

    public int IdMatch { get; set; }

    public virtual AutomaticRecommendation? AutomaticRecommendation { get; set; }

    public virtual ICollection<GeneralEvent> GeneralEvents { get; set; } = new List<GeneralEvent>();

    public virtual Match IdMatchNavigation { get; set; } = null!;

    public virtual User? IdUserNavigation { get; set; }

    public virtual ICollection<PersonalEvent> PersonalEvents { get; set; } = new List<PersonalEvent>();

    public virtual ICollection<TeamEvent> TeamEvents { get; set; } = new List<TeamEvent>();

    public virtual ICollection<TeamMemberMatch> TeamMemberMatches { get; set; } = new List<TeamMemberMatch>();
}
