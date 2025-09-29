using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class TeamMemberRepository : ITeamMemberRepository
    {
        private readonly MatchDbContext _context;

        public TeamMemberRepository(MatchDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TeamMember> GetAll()
        {
            return _context.TeamMembers
                .Include(tm => tm.IdPlayerNavigation)
                    .ThenInclude(p => p.IdPositionNavigation)
                .Include(tm => tm.IdPlayerNavigation)
                    .ThenInclude(p => p.IdNationalityNavigation)
                .Include(tm => tm.IdTeamNavigation)
                .ToList();
        }

        public TeamMember? GetById(int idPlayer, int idTeam)
        {
            return _context.TeamMembers
                .Include(tm => tm.IdPlayerNavigation)
                    .ThenInclude(p => p.IdPositionNavigation)
                .Include(tm => tm.IdPlayerNavigation)
                    .ThenInclude(p => p.IdNationalityNavigation)
                .Include(tm => tm.IdTeamNavigation)
                .FirstOrDefault(tm => tm.IdPlayer == idPlayer && tm.IdTeam == idTeam);
        }

        public TeamMember Create(TeamMember teamMember)
        {
            _context.TeamMembers.Add(teamMember);
            _context.SaveChanges();
            return teamMember;
        }

        public TeamMember? Update(TeamMember teamMember)
        {
            var existingTeamMember = _context.TeamMembers
                .FirstOrDefault(tm => tm.IdPlayer == teamMember.IdPlayer && tm.IdTeam == teamMember.IdTeam);
            
            if (existingTeamMember == null)
                return null;

            existingTeamMember.JerseyNumber = teamMember.JerseyNumber;
            existingTeamMember.Status = teamMember.Status;

            _context.SaveChanges();
            return existingTeamMember;
        }

        public bool Delete(int idPlayer, int idTeam)
        {
            var teamMember = _context.TeamMembers
                .FirstOrDefault(tm => tm.IdPlayer == idPlayer && tm.IdTeam == idTeam);
            
            if (teamMember == null)
                return false;

            _context.TeamMembers.Remove(teamMember);
            _context.SaveChanges();
            return true;
        }

        public bool Exists(int idPlayer, int idTeam)
        {
            return _context.TeamMembers
                .Any(tm => tm.IdPlayer == idPlayer && tm.IdTeam == idTeam);
        }

        public IEnumerable<TeamMember> GetByTeamId(int idTeam)
        {
            return _context.TeamMembers
                .Include(tm => tm.IdPlayerNavigation)
                    .ThenInclude(p => p.IdPositionNavigation)
                .Include(tm => tm.IdPlayerNavigation)
                    .ThenInclude(p => p.IdNationalityNavigation)
                .Where(tm => tm.IdTeam == idTeam)
                .ToList();
        }

        public IEnumerable<TeamMember> GetByPlayerId(int idPlayer)
        {
            return _context.TeamMembers
                .Include(tm => tm.IdTeamNavigation)
                .Where(tm => tm.IdPlayer == idPlayer)
                .ToList();
        }
    }
}
