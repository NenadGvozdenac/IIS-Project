using System;
using System.Collections.Generic;

namespace travel_service.src.Travels.Core.Domain.Entities;

public partial class OfferSelectionLog
{
    public int IdSelection { get; set; }

    public int IdMatch { get; set; }

    public string OfferType { get; set; } = null!;

    public int? SelectedOfferId { get; set; }

    public decimal? SelectionScore { get; set; }

    public string? SelectionReason { get; set; }

    public int? TotalOffersAnalyzed { get; set; }

    public DateTime? SelectedAt { get; set; }

    public string? SelectedBy { get; set; }
}
