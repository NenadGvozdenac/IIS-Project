using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class TransportationRequest
{
    public int IdRequest { get; set; }

    public int? NumberOfPassengers { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? VehicleType { get; set; }

    public virtual Request IdRequestNavigation { get; set; } = null!;
}
