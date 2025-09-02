namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetIndividualTickets;

public class GetIndividualTicketsResponse
{
    public int IdPurchaseOffer { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateOnly ReleasedAt { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly? ExpiresAt { get; set; }
    public int IdSeat { get; set; }
    public int IdMatch { get; set; }
    public decimal Price { get; set; }
    
    // Seat information
    public int SeatRow { get; set; }
    public int SeatNumber { get; set; }
    public string SeatType { get; set; } = null!;
    public string SeatDirection { get; set; } = null!;
    public string SeatStatus { get; set; } = null!;
    
    // Zone information
    public string? ZoneName { get; set; }
    public int? ZoneRank { get; set; }
    
    // Match information
    public string MatchName { get; set; } = null!;
}
