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

    public User? GetById(int id)
    {
        return _matchDbContext.Users.Find(id);
    }

    public User Create(User user)
    {
        _matchDbContext.Users.Add(user);
        _matchDbContext.SaveChanges();
        return user;
    }

    public User? GetByEmail(string email)
    {
        return _matchDbContext.Users.FirstOrDefault(u => u.Email == email);
    }
}
