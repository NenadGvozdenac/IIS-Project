using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Relational;

public class CreditCardRepository : ICreditCardRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public CreditCardRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public CreditCard? GetById(int id)
    {
        return _ticketDbContext.CreditCards
            .Include(cc => cc.IdUserNavigation)
            .FirstOrDefault(cc => cc.IdCreditCard == id);
    }

    public IEnumerable<CreditCard> GetAll()
    {
        return _ticketDbContext.CreditCards
            .Include(cc => cc.IdUserNavigation)
            .ToList();
    }

    public IEnumerable<CreditCard> GetByUserId(int userId)
    {
        return _ticketDbContext.CreditCards
            .Include(cc => cc.IdUserNavigation)
            .Where(cc => cc.IdUser == userId)
            .ToList();
    }

    public CreditCard Create(CreditCard creditCard)
    {
        _ticketDbContext.CreditCards.Add(creditCard);
        _ticketDbContext.SaveChanges();
        return creditCard;
    }

    public CreditCard Update(CreditCard creditCard)
    {
        _ticketDbContext.CreditCards.Update(creditCard);
        _ticketDbContext.SaveChanges();
        return creditCard;
    }

    public bool Delete(int id)
    {
        var creditCard = _ticketDbContext.CreditCards.Find(id);
        if (creditCard == null) return false;

        _ticketDbContext.CreditCards.Remove(creditCard);
        _ticketDbContext.SaveChanges();
        return true;
    }
}
