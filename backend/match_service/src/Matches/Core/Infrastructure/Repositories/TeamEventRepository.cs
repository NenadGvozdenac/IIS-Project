using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class TeamEventRepository : ITeamEventRepository
    {
        private readonly MatchDbContext _context;

        public TeamEventRepository(MatchDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TeamEvent> GetByMatchId(int matchId)
        {
            return _context.TeamEvents
                .Include(te => te.IdTeamNavigation)
                .Include(te => te.IdMatchNavigation)
                .Where(te => te.IdMatch == matchId)
                .OrderBy(te => te.CreationTime)
                .ToList();
        }

        public IEnumerable<TeamEvent> GetByTeamId(int teamId)
        {
            return _context.TeamEvents
                .Include(te => te.IdTeamNavigation)
                .Include(te => te.IdMatchNavigation)
                .Where(te => te.IdTeam == teamId)
                .OrderBy(te => te.CreationTime)
                .ToList();
        }

        public TeamEvent? GetById(int eventId)
        {
            return _context.TeamEvents
                .Include(te => te.IdTeamNavigation)
                .Include(te => te.IdMatchNavigation)
                .FirstOrDefault(te => te.IdEvent == eventId);
        }

        public TeamEvent Create(TeamEvent teamEvent)
        {
            _context.TeamEvents.Add(teamEvent);
            _context.SaveChanges();
            return teamEvent;
        }

        public void Update(TeamEvent teamEvent)
        {
            _context.TeamEvents.Update(teamEvent);
            _context.SaveChanges();
        }

        public bool Delete(int eventId)
        {
            var teamEvent = GetById(eventId);
            if (teamEvent == null)
                return false;

            _context.TeamEvents.Remove(teamEvent);
            _context.SaveChanges();
            return true;
        }

        public bool Exists(int eventId)
        {
            return _context.TeamEvents
                .Any(te => te.IdEvent == eventId);
        }
    }
}