using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class MatchZoneSalesSummary
{
    public int IdSummary { get; set; }

    public int IdMatch { get; set; }

    public int IdZone { get; set; }

    public int? IdTicketPriceParameter { get; set; }

    public int TotalTicketsSold { get; set; }

    public decimal TotalRevenue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Match IdMatchNavigation { get; set; } = null!;

    public virtual TicketPriceParameter? IdTicketPriceParameterNavigation { get; set; }

    public virtual Zone IdZoneNavigation { get; set; } = null!;
}
