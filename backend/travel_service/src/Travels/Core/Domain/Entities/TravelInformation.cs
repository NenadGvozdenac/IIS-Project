using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class TravelInformation
{
    public int IdTravelInformation { get; set; }

    public string? PassportNumber { get; set; }

    public DateOnly? PassportExpirationDate { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }

    public int? IdManagementMember { get; set; }

    public int? IdTeam { get; set; }

    public int? IdPlayer { get; set; }

    public virtual TeamMember? Id { get; set; }

    public virtual Management? IdManagementMemberNavigation { get; set; }

    public virtual ICollection<Visa> Visas { get; set; } = new List<Visa>();
}
