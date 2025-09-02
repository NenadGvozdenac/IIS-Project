using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class Visa
{
    public string VisaNumber { get; set; } = null!;

    public string? State { get; set; }

    public DateOnly? CreationDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public int IdTravelInformation { get; set; }

    public virtual TravelInformation IdTravelInformationNavigation { get; set; } = null!;
}
