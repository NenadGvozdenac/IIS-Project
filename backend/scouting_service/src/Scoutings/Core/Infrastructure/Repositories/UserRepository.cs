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

    public User? GetById(int id)
    {
        return _scoutingDbContext.Users.Find(id);
    }

    public User Create(User user)
    {
        _scoutingDbContext.Users.Add(user);
        _scoutingDbContext.SaveChanges();
        return user;
    }

    public User? GetByEmail(string email)
    {
        return _scoutingDbContext.Users.FirstOrDefault(u => u.Email == email);
    }
}
