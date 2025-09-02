using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Agency
{
    public int IdAgency { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<SentRequest> SentRequests { get; set; } = new List<SentRequest>();
}
