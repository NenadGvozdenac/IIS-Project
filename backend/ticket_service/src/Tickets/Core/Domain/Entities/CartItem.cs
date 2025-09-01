using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class CartItem
{
    public int IdCart { get; set; }

    public int IdPurchaseOffer { get; set; }

    public DateOnly? AddedAt { get; set; }

    public int? Quantity { get; set; }

    public virtual Cart IdCartNavigation { get; set; } = null!;

    public virtual PurchaseOffer IdPurchaseOfferNavigation { get; set; } = null!;
}
