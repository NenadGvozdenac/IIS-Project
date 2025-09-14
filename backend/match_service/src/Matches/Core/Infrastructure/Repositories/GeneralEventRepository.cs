using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class GeneralEventRepository : IGeneralEventRepository
    {
        private readonly MatchDbContext _context;

        public GeneralEventRepository(MatchDbContext context)
        {
            _context = context;
        }

        public IEnumerable<GeneralEvent> GetByMatchId(int matchId)
        {
            return _context.GeneralEvents
                .Include(e => e.IdMatchNavigation)
                .Where(e => e.IdMatch == matchId)
                .OrderBy(e => e.CreationTime)
                .ToList();
        }

        public GeneralEvent? GetById(int eventId)
        {
            return _context.GeneralEvents
                .Include(e => e.IdMatchNavigation)
                .FirstOrDefault(e => e.IdEvent == eventId);
        }

        public GeneralEvent Create(GeneralEvent generalEvent)
        {
            _context.GeneralEvents.Add(generalEvent);
            _context.SaveChanges();
            return generalEvent;
        }

        public void Update(GeneralEvent generalEvent)
        {
            _context.GeneralEvents.Update(generalEvent);
            _context.SaveChanges();
        }

        public bool Delete(int eventId)
        {
            var generalEvent = _context.GeneralEvents.Find(eventId);
            if (generalEvent == null)
                return false;

            _context.GeneralEvents.Remove(generalEvent);
            _context.SaveChanges();
            return true;
        }

        public bool Exists(int eventId)
        {
            return _context.GeneralEvents.Any(e => e.IdEvent == eventId);
        }
    }
}