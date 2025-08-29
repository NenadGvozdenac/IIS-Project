using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.BuildingBlocks.Infratructure.Database;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public UserRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _scoutingDbContext.Users.FindAsync(id);
    }

    public User CreateAsync(User user)
    {
        _scoutingDbContext.Users.Add(user);
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _scoutingDbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _scoutingDbContext.SaveChangesAsync();
    }
}
