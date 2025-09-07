using System;
using System.Collections.Generic;

namespace scouting_service.src.Scoutings.Core.Domain.Entities;

public partial class MetricType
{
    public int IdType { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Metric> Metrics { get; set; } = new List<Metric>();
}
