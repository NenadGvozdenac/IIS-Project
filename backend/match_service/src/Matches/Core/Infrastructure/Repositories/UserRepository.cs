using Microsoft.EntityFrameworkCore;
using match_service.src.Matches.BuildingBlocks.Infratructure.Database;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MatchDbContext _matchDbContext;

    public UserRepository(MatchDbContext context)
    {
        _matchDbContext = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _matchDbContext.Users.FindAsync(id);
    }

    public User CreateAsync(User user)
    {
        _matchDbContext.Users.Add(user);
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _matchDbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _matchDbContext.SaveChangesAsync();
    }
}
