using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface IUserRepository
{
    User? GetById(int id);
    User Create(User user);
    User? GetByEmail(string email);
}
