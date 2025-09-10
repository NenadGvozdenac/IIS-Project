namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetExistingSeasonTickets;

public class GetExistingSeasonTicketsResponse
{
    public int IdPurchaseOffer { get; set; }
    public int IdSeat { get; set; }
    public int SeatRow { get; set; }
    public int SeatNumber { get; set; }
    public string SeatDirection { get; set; } = null!;
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public decimal Price { get; set; }
    public DateOnly CreatedAt { get; set; }
}
