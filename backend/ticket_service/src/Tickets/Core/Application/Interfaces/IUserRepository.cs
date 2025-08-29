using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    User CreateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task SaveChangesAsync();
}
