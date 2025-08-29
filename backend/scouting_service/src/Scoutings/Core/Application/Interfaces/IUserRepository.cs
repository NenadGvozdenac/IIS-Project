using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    User CreateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task SaveChangesAsync();
}
