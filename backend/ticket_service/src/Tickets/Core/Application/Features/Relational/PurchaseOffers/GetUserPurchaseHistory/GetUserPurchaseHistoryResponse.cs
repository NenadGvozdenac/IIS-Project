namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.GetUserPurchaseHistory;

public class GetUserPurchaseHistoryResponse
{
    public int IdPurchaseOffer { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateOnly ReleasedAt { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly? ExpiresAt { get; set; }
    public int IdSeat { get; set; }
    public decimal Price { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public DateOnly? ValidFrom { get; set; } // When the ticket becomes valid (important for season tickets)
    
    // Seat information
    public int SeatRow { get; set; }
    public int SeatNumber { get; set; }
    public string SeatType { get; set; } = null!;
    public string SeatDirection { get; set; } = null!;
    public string SeatStatus { get; set; } = null!;
    
    // Zone information
    public string? ZoneName { get; set; }
    public int? ZoneRank { get; set; }
    
    // Additional info based on ticket type
    public string? SeasonName { get; set; } // For season tickets
    public string? MatchName { get; set; }  // For individual tickets
}
