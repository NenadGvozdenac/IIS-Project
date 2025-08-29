using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    User CreateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task SaveChangesAsync();
}
