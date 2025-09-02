using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class SentRequest
{
    public int IdAgency { get; set; }

    public int IdRequest { get; set; }

    public virtual Agency IdAgencyNavigation { get; set; } = null!;

    public virtual Request IdRequestNavigation { get; set; } = null!;

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
