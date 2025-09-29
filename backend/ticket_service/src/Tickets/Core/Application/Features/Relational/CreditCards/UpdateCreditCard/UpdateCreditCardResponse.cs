namespace ticket_service.src.Tickets.Core.Application.Features.Relational.CreditCards.UpdateCreditCard;

public class UpdateCreditCardResponse
{
    public int IdCreditCard { get; set; }
    public DateOnly? CreatedAt { get; set; }
    public string? Number { get; set; }
    public string? Name { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public int IdUser { get; set; }
}
