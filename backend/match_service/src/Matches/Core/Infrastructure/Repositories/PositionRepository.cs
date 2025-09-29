using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly MatchDbContext _context;

        public PositionRepository(MatchDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Position> GetAll()
        {
            return _context.Positions.ToList();
        }

        public Position? GetById(int id)
        {
            return _context.Positions
                .FirstOrDefault(p => p.IdPosition == id);
        }

        public bool Exists(int id)
        {
            return _context.Positions.Any(p => p.IdPosition == id);
        }
    }
}
