using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.BuildingBlocks.Infratructure.Database;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TravelDbContext _travelDbContext;

    public UserRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _travelDbContext.Users.FindAsync(id);
    }

    public User CreateAsync(User user)
    {
        _travelDbContext.Users.Add(user);
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _travelDbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _travelDbContext.SaveChangesAsync();
    }
}
