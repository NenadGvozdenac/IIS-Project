namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.AddToCart;

public class AddToCartResponse
{
    public int IdCart { get; set; }
    public int IdPurchaseOffer { get; set; }
    public DateOnly AddedAt { get; set; }
    public decimal Price { get; set; }
    public string Message { get; set; } = null!;
}
