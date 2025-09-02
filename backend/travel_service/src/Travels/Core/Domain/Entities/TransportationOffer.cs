using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class TransportationOffer
{
    public int IdOffer { get; set; }

    public string? CompanyName { get; set; }

    public int? Capacity { get; set; }

    public string? Type { get; set; }

    public int IdAgency { get; set; }

    public int IdRequest { get; set; }

    public bool EquipmentSpace { get; set; }

    public bool AirConditioning { get; set; }

    public bool Tv { get; set; }

    public bool Wifi { get; set; }

    public bool Restroom { get; set; }

    public virtual Offer Id { get; set; } = null!;

    public virtual Trip? Trip { get; set; }
}
