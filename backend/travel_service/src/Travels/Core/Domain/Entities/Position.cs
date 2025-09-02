using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Position
{
    public int IdPosition { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
