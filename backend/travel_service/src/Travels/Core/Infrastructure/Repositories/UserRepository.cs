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

    public User? GetById(int id)
    {
        return _travelDbContext.Users.Find(id);
    }

    public User Create(User user)
    {
        _travelDbContext.Users.Add(user);
        _travelDbContext.SaveChanges();
        return user;
    }

    public User? GetByEmail(string email)
    {
        return _travelDbContext.Users.FirstOrDefault(u => u.Email == email);
    }
}
