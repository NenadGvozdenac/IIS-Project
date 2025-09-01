namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetAllCreditCards;

public class GetAllCreditCardsResponse
{
    public int IdCreditCard { get; set; }
    public DateOnly? CreatedAt { get; set; }
    public string? Number { get; set; }
    public int? Cvv { get; set; }
    public string? Name { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public int IdUser { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}
