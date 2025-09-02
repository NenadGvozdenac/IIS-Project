using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class Nationality
{
    public int IdNationality { get; set; }

    public string? State { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
