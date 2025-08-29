using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IUserRepository
{
    User? GetById(int id);
    User Create(User user);
    User? GetByEmail(string email);
}
