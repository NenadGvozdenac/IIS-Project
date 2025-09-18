namespace ticket_service.src.Tickets.Core.Application.Features.Relational.CreditCards.GetCreditCardById;

public class GetCreditCardByIdResponse
{
    public int IdCreditCard { get; set; }
    public DateOnly? CreatedAt { get; set; }
    public string? Number { get; set; }
    public string? Name { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public int IdUser { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}
