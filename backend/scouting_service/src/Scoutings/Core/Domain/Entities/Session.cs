using System;
using System.Collections.Generic;

namespace scouting_service.src.Scoutings.Core.Domain.Entities;

public partial class Session
{
    public int IdSession { get; set; }

    public DateOnly StartTime { get; set; }

    public DateOnly? EndTime { get; set; }

    public int IdSessionStatus { get; set; }

    public int IdSessionType { get; set; }

    public int IdUser { get; set; }

    public int IdPlayer { get; set; }

    public string Note { get; set; } = null!;

    public virtual Player IdPlayerNavigation { get; set; } = null!;

    public virtual SessionStatus IdSessionStatusNavigation { get; set; } = null!;

    public virtual SessionType IdSessionTypeNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<SessionMetric> SessionMetrics { get; set; } = new List<SessionMetric>();
}
