using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Trip
{
    public int IdTrip { get; set; }

    public string? Notes { get; set; }

    public int MatchIdMatch { get; set; }

    public int? IdAccommodationOffer { get; set; }

    public int? IdTransportationOffer { get; set; }

    public int? IdAccommodationAgency { get; set; }

    public int? IdTransportationAgency { get; set; }

    public int? IdAccommodationRequest { get; set; }

    public int? IdTransportationRequest { get; set; }

    public virtual AccommodationOffer? IdAccommodation { get; set; }

    public virtual TransportationOffer? IdTransportation { get; set; }

    public virtual Match MatchIdMatchNavigation { get; set; } = null!;
}
