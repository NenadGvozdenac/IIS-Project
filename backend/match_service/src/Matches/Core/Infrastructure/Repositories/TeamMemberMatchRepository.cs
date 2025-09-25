using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class TeamMemberMatchRepository : ITeamMemberMatchRepository
    {
        private readonly MatchDbContext _context;

        public TeamMemberMatchRepository(MatchDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TeamMemberMatch> GetByMatchId(int matchId)
        {
            return _context.TeamMemberMatches
                .Include(tmm => tmm.Id)
                    .ThenInclude(tm => tm.IdPlayerNavigation)
                        .ThenInclude(p => p.IdPositionNavigation)
                .Include(tmm => tmm.Id)
                    .ThenInclude(tm => tm.IdTeamNavigation)
                .Where(tmm => tmm.IdMatch == matchId)
                .ToList();
        }

        public IEnumerable<TeamMemberMatch> GetByMatchAndTeamId(int matchId, int teamId)
        {
            return _context.TeamMemberMatches
                .Include(tmm => tmm.Id)
                    .ThenInclude(tm => tm.IdPlayerNavigation)
                        .ThenInclude(p => p.IdPositionNavigation)
                .Include(tmm => tmm.Id)
                    .ThenInclude(tm => tm.IdTeamNavigation)
                .Where(tmm => tmm.IdMatch == matchId && tmm.IdTeam == teamId)
                .ToList();
        }

        public TeamMemberMatch? GetById(int matchId, int teamId, int playerId)
        {
            return _context.TeamMemberMatches
                .FirstOrDefault(tmm => tmm.IdMatch == matchId && tmm.IdTeam == teamId && tmm.IdPlayer == playerId);
        }

        public TeamMemberMatch Create(TeamMemberMatch teamMemberMatch)
        {
            _context.TeamMemberMatches.Add(teamMemberMatch);
            _context.SaveChanges();
            return teamMemberMatch;
        }

        public void Update(TeamMemberMatch teamMemberMatch)
        {
            _context.TeamMemberMatches.Update(teamMemberMatch);
            _context.SaveChanges();
        }

        public bool Delete(int matchId, int teamId, int playerId)
        {
            var teamMemberMatch = GetById(matchId, teamId, playerId);
            if (teamMemberMatch == null)
                return false;

            _context.TeamMemberMatches.Remove(teamMemberMatch);
            _context.SaveChanges();
            return true;
        }

        public bool Exists(int matchId, int teamId, int playerId)
        {
            return _context.TeamMemberMatches
                .Any(tmm => tmm.IdMatch == matchId && tmm.IdTeam == teamId && tmm.IdPlayer == playerId);
        }

        public void AddRange(IEnumerable<TeamMemberMatch> teamMemberMatches)
        {
            _context.TeamMemberMatches.AddRange(teamMemberMatches);
        }

        // Saga deletion method
        public int DeleteByPlayerAndTeam(int playerId, int teamId)
        {
            var teamMemberMatches = _context.TeamMemberMatches
                .Where(tmm => tmm.IdPlayer == playerId && tmm.IdTeam == teamId)
                .ToList();

            if (!teamMemberMatches.Any())
                return 0;

            _context.TeamMemberMatches.RemoveRange(teamMemberMatches);
            _context.SaveChanges();
            return teamMemberMatches.Count;
        }
    }
}
