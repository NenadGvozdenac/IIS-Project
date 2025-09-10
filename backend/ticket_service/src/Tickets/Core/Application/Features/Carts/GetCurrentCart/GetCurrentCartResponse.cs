namespace ticket_service.src.Tickets.Core.Application.Features.Carts.GetCurrentCart;

public class GetCurrentCartResponse
{
    public int IdCart { get; set; }
    public DateOnly CreatedAt { get; set; }
    public int ItemsNumber { get; set; }
    public string Status { get; set; } = null!;
    public bool IsCurrent { get; set; }
    public int? IdCreditCard { get; set; }
    public int IdUser { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? CreditCardNumber { get; set; }
    public List<CartItemResponse> CartItems { get; set; } = new List<CartItemResponse>();
    public decimal TotalAmount { get; set; }
}

public class CartItemResponse
{
    public int IdCart { get; set; }
    public int IdPurchaseOffer { get; set; }
    public DateOnly AddedAt { get; set; }
    public decimal Price { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public string? PurchaseOfferName { get; set; }
    public string? PurchaseOfferDescription { get; set; }
}
