using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class AutomaticRecommendationRepository : IAutomaticRecommendationRepository
    {
        private readonly MatchDbContext _context;

        public AutomaticRecommendationRepository(MatchDbContext context)
        {
            _context = context;
        }

        public AutomaticRecommendation Create(AutomaticRecommendation automaticRecommendation)
        {
            _context.AutomaticRecommendations.Add(automaticRecommendation);
            _context.SaveChanges();
            return automaticRecommendation;
        }

        public AutomaticRecommendation? GetById(int id)
        {
            return _context.AutomaticRecommendations
                .Include(ar => ar.IdMatchNavigation)
                .FirstOrDefault(ar => ar.IdRecommendation == id);
        }

        public IEnumerable<AutomaticRecommendation> GetByMatchId(int matchId)
        {
            return _context.AutomaticRecommendations
                .Include(ar => ar.IdMatchNavigation)
                .Where(ar => ar.IdMatch == matchId)
                .OrderBy(ar => ar.CreationTime)
                .ToList();
        }

        public AutomaticRecommendation Update(AutomaticRecommendation automaticRecommendation)
        {
            _context.AutomaticRecommendations.Update(automaticRecommendation);
            _context.SaveChanges();
            return automaticRecommendation;
        }

        public void Delete(int id)
        {
            var recommendation = GetById(id);
            if (recommendation != null)
            {
                _context.AutomaticRecommendations.Remove(recommendation);
                _context.SaveChanges();
            }
        }
    }
}