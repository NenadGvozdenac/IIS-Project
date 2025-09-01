using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface ICreditCardRepository
{
    CreditCard? GetById(int id);
    IEnumerable<CreditCard> GetAll();
    IEnumerable<CreditCard> GetByUserId(int userId);
    CreditCard Create(CreditCard creditCard);
    CreditCard Update(CreditCard creditCard);
    bool Delete(int id);
}
