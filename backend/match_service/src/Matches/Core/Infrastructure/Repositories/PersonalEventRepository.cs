using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class PersonalEventRepository : IPersonalEventRepository
    {
        private readonly MatchDbContext _context;

        public PersonalEventRepository(MatchDbContext context)
        {
            _context = context;
        }

        public IEnumerable<PersonalEvent> GetByMatchId(int matchId)
        {
            return _context.PersonalEvents
                .Include(pe => pe.Id)
                    .ThenInclude(tm => tm.IdPlayerNavigation)
                .Include(pe => pe.Id)
                    .ThenInclude(tm => tm.IdTeamNavigation)
                .Where(pe => pe.IdMatch == matchId)
                .OrderBy(pe => pe.CreationTime)
                .ToList();
        }

        public IEnumerable<PersonalEvent> GetByPlayerId(int teamId, int playerId)
        {
            return _context.PersonalEvents
                .Include(pe => pe.Id)
                    .ThenInclude(tm => tm.IdPlayerNavigation)
                .Include(pe => pe.Id)
                    .ThenInclude(tm => tm.IdTeamNavigation)
                .Where(pe => pe.IdTeam == teamId && pe.IdPlayer == playerId)
                .OrderBy(pe => pe.CreationTime)
                .ToList();
        }

        public PersonalEvent? GetById(int eventId)
        {
            return _context.PersonalEvents
                .Include(pe => pe.Id)
                    .ThenInclude(tm => tm.IdPlayerNavigation)
                .Include(pe => pe.Id)
                    .ThenInclude(tm => tm.IdTeamNavigation)
                .FirstOrDefault(pe => pe.IdEvent == eventId);
        }

        public PersonalEvent? GetLastEventByTeamAndMatch(int matchId, int teamId)
        {
            return _context.PersonalEvents
                .Include(pe => pe.Id)
                    .ThenInclude(tm => tm.IdPlayerNavigation)
                .Include(pe => pe.Id)
                    .ThenInclude(tm => tm.IdTeamNavigation)
                .Where(pe => pe.IdMatch == matchId && pe.IdTeam == teamId)
                .OrderByDescending(pe => pe.CreationTime)
                .FirstOrDefault();
        }

        public PersonalEvent Create(PersonalEvent personalEvent)
        {
            _context.PersonalEvents.Add(personalEvent);
            _context.SaveChanges();
            return personalEvent;
        }

        public void Update(PersonalEvent personalEvent)
        {
            _context.PersonalEvents.Update(personalEvent);
            _context.SaveChanges();
        }

        public bool Delete(int eventId)
        {
            var personalEvent = GetById(eventId);
            if (personalEvent == null)
                return false;

            _context.PersonalEvents.Remove(personalEvent);
            _context.SaveChanges();
            return true;
        }

        public bool Exists(int eventId)
        {
            return _context.PersonalEvents
                .Any(pe => pe.IdEvent == eventId);
        }

        // Saga deletion method
        public int DeleteByPlayerAndTeam(int playerId, int teamId)
        {
            var personalEvents = _context.PersonalEvents
                .Where(pe => pe.IdPlayer == playerId && pe.IdTeam == teamId)
                .ToList();

            if (!personalEvents.Any())
                return 0;

            _context.PersonalEvents.RemoveRange(personalEvents);
            _context.SaveChanges();
            return personalEvents.Count;
        }

        // SAGA BACKUP METHOD - VRAĆA PODATKE PRE BRISANJA
        public List<PersonalEvent> GetByPlayerAndTeam(int playerId, int teamId)
        {
            return _context.PersonalEvents
                .Include(pe => pe.Id) // TeamMember navigation
                .Include(pe => pe.IdMatchNavigation)
                .Where(pe => pe.IdPlayer == playerId && pe.IdTeam == teamId)
                .ToList();
        }
    }
}