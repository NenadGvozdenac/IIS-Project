using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphMatchRepository
{
    public Task CreateMatch(Match match);
    public Task<Match?> GetMatchByName(string name);
    public Task<List<Match>> GetAllMatches();
    public Task UpdateMatch(Match match);
    public Task DeleteMatch(string name);
    public Task<List<Match>> GetMatchesByDateRange(DateTime startDate, DateTime endDate);
}