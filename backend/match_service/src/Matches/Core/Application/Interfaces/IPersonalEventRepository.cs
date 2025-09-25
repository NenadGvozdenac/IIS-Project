using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces
{
    public interface IPersonalEventRepository
    {
        IEnumerable<PersonalEvent> GetByMatchId(int matchId);
        IEnumerable<PersonalEvent> GetByPlayerId(int teamId, int playerId);
        PersonalEvent? GetById(int eventId);
        PersonalEvent? GetLastEventByTeamAndMatch(int matchId, int teamId);
        PersonalEvent Create(PersonalEvent personalEvent);
        void Update(PersonalEvent personalEvent);
        bool Delete(int eventId);
        bool Exists(int eventId);
        
        // Saga deletion methods
        int DeleteByPlayerAndTeam(int playerId, int teamId);
    }
}