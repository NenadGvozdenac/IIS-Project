namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetCreditCardsByUser;

public class GetCreditCardsByUserResponse
{
    public int IdCreditCard { get; set; }
    public DateOnly? CreatedAt { get; set; }
    public string? Number { get; set; } // Masked card number
    public string? Name { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public int IdUser { get; set; }
}
