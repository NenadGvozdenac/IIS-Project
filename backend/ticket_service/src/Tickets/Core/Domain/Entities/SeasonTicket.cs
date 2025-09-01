using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class SeasonTicket
{
    public int IdPurchaseOffer { get; set; }

    public int IdSeason { get; set; }

    public int FixedPromotionalTicketPrice { get; set; }

    public virtual Season IdSeasonNavigation { get; set; } = null!;
}
