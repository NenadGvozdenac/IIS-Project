using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    User CreateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task SaveChangesAsync();
}
