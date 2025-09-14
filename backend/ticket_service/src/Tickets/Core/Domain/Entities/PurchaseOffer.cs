using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class PurchaseOffer
{
    public int IdPurchaseOffer { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateOnly ReleasedAt { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly? ExpiresAt { get; set; }

    public int IdSeat { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Seat IdSeatNavigation { get; set; } = null!;

    public virtual IndividualTicket? IndividualTicket { get; set; }

    public virtual SeasonTicket? SeasonTicket { get; set; }
}
