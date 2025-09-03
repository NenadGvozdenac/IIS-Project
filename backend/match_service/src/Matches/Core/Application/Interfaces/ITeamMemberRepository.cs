using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces
{
    public interface ITeamMemberRepository
    {
        IEnumerable<TeamMember> GetAll();
        TeamMember? GetById(int idPlayer, int idTeam);
        TeamMember Create(TeamMember teamMember);
        TeamMember? Update(TeamMember teamMember);
        bool Delete(int idPlayer, int idTeam);
        bool Exists(int idPlayer, int idTeam);
        IEnumerable<TeamMember> GetByTeamId(int idTeam);
        IEnumerable<TeamMember> GetByPlayerId(int idPlayer);
    }
}
