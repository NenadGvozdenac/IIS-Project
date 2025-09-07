using System;
using System.Collections.Generic;

namespace scouting_service.src.Scoutings.Core.Domain.Entities;

public partial class SessionStatus
{
    public int IdStatus { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
