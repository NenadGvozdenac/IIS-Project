using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class TeamMember
{
    public int? JerseyNumber { get; set; }

    public string? Status { get; set; }

    public int IdPlayer { get; set; }

    public int IdTeam { get; set; }

    public virtual Player IdPlayerNavigation { get; set; } = null!;

    public virtual Team IdTeamNavigation { get; set; } = null!;

    public virtual TravelInformation? TravelInformation { get; set; }
}
