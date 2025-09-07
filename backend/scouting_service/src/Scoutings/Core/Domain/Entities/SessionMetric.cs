using System;
using System.Collections.Generic;

namespace scouting_service.src.Scoutings.Core.Domain.Entities;

public partial class SessionMetric
{
    public string? Value { get; set; }

    public int IdSession { get; set; }

    public int IdMetrics { get; set; }

    public virtual Metric IdMetricsNavigation { get; set; } = null!;

    public virtual Session IdSessionNavigation { get; set; } = null!;
}
