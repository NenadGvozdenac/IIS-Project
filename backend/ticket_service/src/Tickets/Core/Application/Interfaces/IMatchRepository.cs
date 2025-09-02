using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface IMatchRepository
{
    IEnumerable<Match> GetAll();
    Match? GetById(int id);
    IEnumerable<Match> GetMatchesInOurHall();
}
