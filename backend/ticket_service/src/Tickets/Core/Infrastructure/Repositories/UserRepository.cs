using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.BuildingBlocks.Infratructure.Database;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public UserRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _ticketDbContext.Users.FindAsync(id);
    }

    public User CreateAsync(User user)
    {
        _ticketDbContext.Users.Add(user);
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _ticketDbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _ticketDbContext.SaveChangesAsync();
    }
}
