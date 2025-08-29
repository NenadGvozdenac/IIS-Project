using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces;

public interface IUserRepository
{
    User? GetById(int id);
    User Create(User user);
    User? GetByEmail(string email);
}
