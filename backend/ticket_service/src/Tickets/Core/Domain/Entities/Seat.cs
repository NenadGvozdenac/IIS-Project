using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class Seat
{
    public int IdSeat { get; set; }

    public int? Row { get; set; }

    public int? Number { get; set; }

    public string? Type { get; set; }

    public string? Direction { get; set; }

    public string? Status { get; set; }

    public int? IdZone { get; set; }

    public virtual Zone? IdZoneNavigation { get; set; }

    public virtual ICollection<PurchaseOffer> PurchaseOffers { get; set; } = new List<PurchaseOffer>();
}
