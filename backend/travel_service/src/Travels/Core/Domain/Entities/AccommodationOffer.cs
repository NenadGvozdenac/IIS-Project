using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class AccommodationOffer
{
    public int IdOffer { get; set; }

    public string? Name { get; set; }

    public int? Capacity { get; set; }

    public string? Type { get; set; }

    public int IdAgency { get; set; }

    public int IdRequest { get; set; }

    public bool DoubleRoom { get; set; }

    public bool TripleRoom { get; set; }

    public bool QuadrupleRoom { get; set; }

    public bool Breakfast { get; set; }

    public bool FitnessCenter { get; set; }

    public bool Pool { get; set; }

    public bool Wifi { get; set; }

    public bool Spa { get; set; }

    public virtual Offer Id { get; set; } = null!;

    public virtual Trip? Trip { get; set; }
}
