using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface IMatchRepository
{
    IEnumerable<Match> GetAll();
    Match? GetById(int id);
    IEnumerable<Match> GetMatchesInOurHall();
    IEnumerable<Match> GetUpcomingMatchesWithinDays(int days);
    void Update(Match match);
}
