using System;
using System.Collections.Generic;

namespace scouting_service.src.Scoutings.Core.Domain.Entities;

public partial class User
{
    public int IdUser { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Password { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual Metric? Metric { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
