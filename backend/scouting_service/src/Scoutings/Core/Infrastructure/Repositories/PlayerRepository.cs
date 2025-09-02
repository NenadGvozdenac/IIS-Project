using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public PlayerRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public Player? GetById(int id)
    {
        return _scoutingDbContext.Players
            .Include(p => p.IdNationalityNavigation)
            .Include(p => p.IdPositionNavigation)
            .Include(p => p.PhysicalMetrics)
            .Include(p => p.Sessions)
            .FirstOrDefault(p => p.IdPlayer == id);
    }

    public IEnumerable<Player> GetAll()
    {
        return _scoutingDbContext.Players
            .Include(p => p.IdNationalityNavigation)
            .Include(p => p.IdPositionNavigation)
            .Include(p => p.PhysicalMetrics)
            .Include(p => p.Sessions)
            .ToList();
    }

    public IEnumerable<Player> GetByNationalityId(int nationalityId)
    {
        return _scoutingDbContext.Players
            .Include(p => p.IdNationalityNavigation)
            .Include(p => p.IdPositionNavigation)
            .Include(p => p.PhysicalMetrics)
            .Include(p => p.Sessions)
            .Where(p => p.IdNationality == nationalityId)
            .ToList();
    }

    public IEnumerable<Player> GetByPositionId(int positionId)
    {
        return _scoutingDbContext.Players
            .Include(p => p.IdNationalityNavigation)
            .Include(p => p.IdPositionNavigation)
            .Include(p => p.PhysicalMetrics)
            .Include(p => p.Sessions)
            .Where(p => p.IdPosition == positionId)
            .ToList();
    }

    public Player Create(Player player)
    {
        _scoutingDbContext.Players.Add(player);
        _scoutingDbContext.SaveChanges();
        return player;
    }

    public Player Update(Player player)
    {
        _scoutingDbContext.Players.Update(player);
        _scoutingDbContext.SaveChanges();
        return player;
    }

    public bool Delete(int id)
    {
        var player = _scoutingDbContext.Players.Find(id);
        if (player == null) return false;

        _scoutingDbContext.Players.Remove(player);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
