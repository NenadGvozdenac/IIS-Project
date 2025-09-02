namespace ticket_service.src.Tickets.Core.Application.Features.Carts.PurchaseCart;

public class PurchaseCartResponse
{
    public int IdCart { get; set; }
    public string Status { get; set; } = null!;
    public int IdCreditCard { get; set; }
    public DateOnly PurchasedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string Message { get; set; } = null!;
}
