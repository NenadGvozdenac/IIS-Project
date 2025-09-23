using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Management
{
    public int MemberId { get; set; }

    public string? MemberName { get; set; }

    public string? MemberSurname { get; set; }

    public string? MemberRole { get; set; }

    public virtual TravelInformation? TravelInformation { get; set; }

    public virtual ICollection<Request> IdRequests { get; set; } = new List<Request>();
}
