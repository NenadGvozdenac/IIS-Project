using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IUserRepository
{
    User? GetById(int id);
    User Create(User user);
    User? GetByEmail(string email);
}
