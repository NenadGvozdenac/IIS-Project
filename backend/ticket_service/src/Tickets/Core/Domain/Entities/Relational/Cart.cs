using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities.Relational;

public partial class Cart
{
    public int IdCart { get; set; }

    public DateOnly CreatedAt { get; set; }

    public int ItemsNumber { get; set; }

    public string Status { get; set; } = null!;

    public bool IsCurrent { get; set; }

    public int? IdCreditCard { get; set; }

    public int IdUser { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual CreditCard? IdCreditCardNavigation { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
