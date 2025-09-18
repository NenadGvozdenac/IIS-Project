using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities.Relational;

public partial class CreditCard
{
    public int IdCreditCard { get; set; }

    public DateOnly CreatedAt { get; set; }

    public string Number { get; set; } = null!;

    public string Cvv { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly ExpirationDate { get; set; }

    public int IdUser { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual User IdUserNavigation { get; set; } = null!;
}
