using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities.Relational;

public partial class IndividualTicket
{
    public int IdPurchaseOffer { get; set; }

    public int IdMatch { get; set; }

    public int IdIndividualTicket { get; set; }

    public virtual Match IdMatchNavigation { get; set; } = null!;

    public virtual PurchaseOffer IdPurchaseOfferNavigation { get; set; } = null!;
}
