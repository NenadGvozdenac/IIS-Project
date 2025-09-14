using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class Season
{
    public int IdSeason { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly StartedAt { get; set; }

    public DateOnly? EndedAt { get; set; }

    public bool TicketsForSale { get; set; }

    public DateTime? TicketsWentOnSale { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<SeasonTicket> SeasonTickets { get; set; } = new List<SeasonTicket>();
}
