using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class TicketPriceParameter
{
    public int IdTicketPriceParameter { get; set; }

    public int PriceFactor { get; set; }

    public int TimeFactor { get; set; }

    public int MinimumSeatPrice { get; set; }

    public int MaximumSeatPrice { get; set; }

    public int IdUser { get; set; }

    public int IdZone { get; set; }

    public int IdMatch { get; set; }

    public virtual Match IdMatchNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual Zone IdZoneNavigation { get; set; } = null!;
}
