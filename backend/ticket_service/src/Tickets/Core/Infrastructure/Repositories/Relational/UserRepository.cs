using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Relational;

public class UserRepository : IUserRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public UserRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public User? GetById(int id)
    {
        return _ticketDbContext.Users.Find(id);
    }

    public User Create(User user)
    {
        _ticketDbContext.Users.Add(user);
        _ticketDbContext.SaveChanges();
        return user;
    }

    public User? GetByEmail(string email)
    {
        return _ticketDbContext.Users.FirstOrDefault(u => u.Email == email);
    }
}
