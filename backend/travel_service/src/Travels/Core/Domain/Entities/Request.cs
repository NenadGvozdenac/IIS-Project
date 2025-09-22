using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Request
{
    public int IdRequest { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public string? Hall { get; set; }

    public int? Budget { get; set; }

    public int IdMatch { get; set; }

    public string? Type { get; set; }

    public virtual AccommodationRequest? AccommodationRequest { get; set; }

    public virtual Match IdMatchNavigation { get; set; } = null!;

    public virtual ICollection<SentRequest> SentRequests { get; set; } = new List<SentRequest>();

    public virtual TransportationRequest? TransportationRequest { get; set; }

    public virtual ICollection<Management> IdManagementMembers { get; set; } = new List<Management>();

    public virtual ICollection<TeamMember> Ids { get; set; } = new List<TeamMember>();
}
