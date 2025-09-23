using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly ScoutingDbContext _context;

    public PlayerRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Player> GetAll()
    {
        return _context.Players
                        .Include(p => p.PhysicalMetrics)
                        .Include(p => p.IdNationalityNavigation)
                        .Include(p => p.IdPositionNavigation)
                        .ToList();
    }

    public Player? GetById(int id)
    {
        return _context.Players.Include(p => p.PhysicalMetrics)
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

    public Player Update(Player player)
    {
        _context.Players.Update(player);
        _context.SaveChanges();
        return player;
    }

    public void Delete(int id)
    {
        var player = _context.Players.Find(id);
        if (player != null)
        {
            _context.Players.Remove(player);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Player> GetByNationality(int nationalityId)
    {
        return _context.Players.Where(p => p.IdNationality == nationalityId).ToList();
    }

    public IEnumerable<Player> GetByPosition(int positionId)
    {
        return _context.Players.Where(p => p.IdPosition == positionId).ToList();
    }
}
