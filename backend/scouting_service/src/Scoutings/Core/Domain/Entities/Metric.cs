using System;
using System.Collections.Generic;

namespace scouting_service.src.Scoutings.Core.Domain.Entities;

public partial class Metric
{
    public int IdMetrics { get; set; }

    public string? Name { get; set; }

    public int? IsPermanent { get; set; }

    public int? MetricWeight { get; set; }

    public int IdUser { get; set; }

    public int IdMetricType { get; set; }

    public virtual MetricType IdMetricTypeNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<SessionMetric> SessionMetrics { get; set; } = new List<SessionMetric>();
}
