using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class Match
{
    public int IdMatch { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public string Type { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Hall { get; set; } = null!;

    public int IsInOurHall { get; set; }

    public int TransportationRequired { get; set; }

    public int AccommodationRequired { get; set; }

    public int? IdCompetition { get; set; }

    public int IdSeason { get; set; }

    public int IdTeam { get; set; }

    public virtual Competition? IdCompetitionNavigation { get; set; }

    public virtual Season IdSeasonNavigation { get; set; } = null!;

    public virtual Team IdTeamNavigation { get; set; } = null!;

    public virtual ICollection<IndividualTicket> IndividualTickets { get; set; } = new List<IndividualTicket>();

    public virtual ICollection<TicketPriceParameter> TicketPriceParameters { get; set; } = new List<TicketPriceParameter>();
}
