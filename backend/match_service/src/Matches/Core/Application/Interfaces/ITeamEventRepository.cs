using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces
{
    public interface ITeamEventRepository
    {
        IEnumerable<TeamEvent> GetByMatchId(int matchId);
        IEnumerable<TeamEvent> GetByTeamId(int teamId);
        TeamEvent? GetById(int eventId);
        TeamEvent Create(TeamEvent teamEvent);
        void Update(TeamEvent teamEvent);
        bool Delete(int eventId);
        bool Exists(int eventId);
    }
}