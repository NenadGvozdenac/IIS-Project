using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphMatchRepository
{
    public Task<Match?> CreateMatch(Match match);
    public Task<Match?> GetMatchByName(string name);
    public Task<Match?> GetMatchById(int id);
    public Task<List<Match>> GetAllMatches();
    public Task<Match?> UpdateMatch(int id, Match match);
    public Task DeleteMatch(int id);
}