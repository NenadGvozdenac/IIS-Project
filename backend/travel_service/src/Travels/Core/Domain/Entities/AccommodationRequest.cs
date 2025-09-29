using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class AccommodationRequest
{
    public int IdRequest { get; set; }

    public int? NumberOfGuests { get; set; }

    public int? NumberOfRooms { get; set; }

    public DateOnly? CheckInDate { get; set; }

    public DateOnly? CheckOutDate { get; set; }

    public string? AccommodationType { get; set; }

    public virtual Request IdRequestNavigation { get; set; } = null!;
}
