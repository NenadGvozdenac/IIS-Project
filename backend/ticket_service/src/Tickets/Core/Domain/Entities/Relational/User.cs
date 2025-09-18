using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities.Relational;

public partial class User
{
    public int IdUser { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Password { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();

    public virtual ICollection<TicketPriceParameter> TicketPriceParameters { get; set; } = new List<TicketPriceParameter>();
}
