using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities;

public partial class CreditCard
{
    public int IdCreditCard { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public string? Number { get; set; }

    public string? Cvv { get; set; }

    public string? Name { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public int IdUser { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual User IdUserNavigation { get; set; } = null!;
}
