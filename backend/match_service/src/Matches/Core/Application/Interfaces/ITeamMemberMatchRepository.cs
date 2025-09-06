using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces
{
    public interface ITeamMemberMatchRepository
    {
        IEnumerable<TeamMemberMatch> GetByMatchId(int matchId);
        TeamMemberMatch? GetById(int matchId, int teamId, int playerId);
        TeamMemberMatch Create(TeamMemberMatch teamMemberMatch);
        void Update(TeamMemberMatch teamMemberMatch);
        bool Delete(int matchId, int teamId, int playerId);
        bool Exists(int matchId, int teamId, int playerId);
        void AddRange(IEnumerable<TeamMemberMatch> teamMemberMatches);
    }
}
