using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly MatchDbContext _context;

        public PlayerRepository(MatchDbContext context)
        {
            _context = context;
        }

        public Player? GetById(int id)
        {
            return _context.Players
                .Include(p => p.IdNationalityNavigation)
                .Include(p => p.IdPositionNavigation)
                .FirstOrDefault(p => p.IdPlayer == id);
        }

        public Player Create(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();
            return player;
        }

        public bool Exists(int id)
        {
            return _context.Players.Any(p => p.IdPlayer == id);
        }
    }
}
