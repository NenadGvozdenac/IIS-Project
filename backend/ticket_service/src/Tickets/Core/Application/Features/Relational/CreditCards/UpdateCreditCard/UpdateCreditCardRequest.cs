namespace ticket_service.src.Tickets.Core.Application.Features.Relational.CreditCards.UpdateCreditCard;

public class UpdateCreditCardRequest
{
    public string? Number { get; set; }
    public int? Cvv { get; set; }
    public string? Name { get; set; }
    public DateOnly? ExpirationDate { get; set; }
}
