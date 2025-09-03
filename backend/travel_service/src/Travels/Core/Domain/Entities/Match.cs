using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Match
{
    public int IdMatch { get; set; }

    public string Name { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }

    public string Type { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Hall { get; set; } = null!;

    public bool IsInOurHall { get; set; }

    public bool TicketsForSale { get; set; }

    public DateTime? TicketsWentOnSale { get; set; }

    public bool TransportationRequired { get; set; }

    public bool AccommodationRequired { get; set; }

    public int? IdCompetition { get; set; }

    public int IdSeason { get; set; }

    public int IdTeam { get; set; }

    public virtual Competition? IdCompetitionNavigation { get; set; }

    public virtual Season IdSeasonNavigation { get; set; } = null!;

    public virtual Team IdTeamNavigation { get; set; } = null!;

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Trip? Trip { get; set; }
}
