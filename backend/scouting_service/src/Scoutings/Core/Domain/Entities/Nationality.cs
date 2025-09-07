using System;
using System.Collections.Generic;

namespace scouting_service.src.Scoutings.Core.Domain.Entities;

public partial class Nationality
{
    public int IdNationality { get; set; }

    public string? State { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
