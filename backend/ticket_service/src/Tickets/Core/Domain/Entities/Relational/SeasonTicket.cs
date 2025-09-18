using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities.Relational;

public partial class SeasonTicket
{
    public int IdPurchaseOffer { get; set; }

    public int IdSeason { get; set; }

    public int TicketPrice { get; set; }

    public virtual PurchaseOffer IdPurchaseOfferNavigation { get; set; } = null!;

    public virtual Season IdSeasonNavigation { get; set; } = null!;
}
