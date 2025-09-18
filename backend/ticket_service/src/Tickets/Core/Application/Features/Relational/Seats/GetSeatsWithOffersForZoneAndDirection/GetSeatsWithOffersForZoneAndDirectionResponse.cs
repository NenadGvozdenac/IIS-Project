namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatsWithOffersForZoneAndDirection;

public class GetSeatsWithOffersForZoneAndDirectionResponse
{
    public int IdSeat { get; set; }
    public int SeatRow { get; set; }
    public int SeatNumber { get; set; }
    public string SeatType { get; set; } = "";
    public string SeatDirection { get; set; } = "";
    public string SeatStatus { get; set; } = "";
    public int IdZone { get; set; }
    public string? ZoneName { get; set; }
    
    // Offer information
    public bool HasOffer { get; set; }
    public bool HasSeasonTicketConflict { get; set; }
    public string? ConflictReason { get; set; }
    
    // Offer details (when available)
    public int? IdPurchaseOffer { get; set; }
    public string? OfferName { get; set; }
    public string? OfferDescription { get; set; }
    public string? OfferStatus { get; set; }
    public DateOnly? ReleasedAt { get; set; }
    public DateOnly? ExpiresAt { get; set; }
    public decimal? Price { get; set; }
    public string? MatchName { get; set; }
}