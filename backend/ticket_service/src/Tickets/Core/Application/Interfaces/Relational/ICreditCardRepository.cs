using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface ICreditCardRepository
{
    CreditCard? GetById(int id);
    IEnumerable<CreditCard> GetAll();
    IEnumerable<CreditCard> GetByUserId(int userId);
    CreditCard Create(CreditCard creditCard);
    CreditCard Update(CreditCard creditCard);
    bool Delete(int id);
}
