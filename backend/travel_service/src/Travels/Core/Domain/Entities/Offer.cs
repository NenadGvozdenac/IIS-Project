using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Offer
{
    public int IdOffer { get; set; }

    public int? Price { get; set; }

    public int? UserIdUser { get; set; }

    public int IdMatch { get; set; }

    public int IdAgency { get; set; }

    public int IdRequest { get; set; }

    public string? Type { get; set; }

    public virtual AccommodationOffer? AccommodationOffer { get; set; }

    public virtual SentRequest Id { get; set; } = null!;

    public virtual Match IdMatchNavigation { get; set; } = null!;

    public virtual TransportationOffer? TransportationOffer { get; set; }

    public virtual User? UserIdUserNavigation { get; set; }
}
