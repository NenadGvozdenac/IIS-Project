using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class Zone
{
    public int IdZone { get; set; }

    public string? Name { get; set; }

    public int? Rank { get; set; }

    public int? MaximumCapacity { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<TicketPriceParameter> TicketPriceParameters { get; set; } = new List<TicketPriceParameter>();
}
