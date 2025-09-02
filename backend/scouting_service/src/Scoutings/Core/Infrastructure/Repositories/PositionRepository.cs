using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public PositionRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public Position? GetById(int id)
    {
        return _scoutingDbContext.Positions
            .Include(p => p.Players)
            .FirstOrDefault(p => p.IdPosition == id);
    }

    public IEnumerable<Position> GetAll()
    {
        return _scoutingDbContext.Positions
            .Include(p => p.Players)
            .ToList();
    }

    public Position? GetByName(string name)
    {
        return _scoutingDbContext.Positions
            .Include(p => p.Players)
            .FirstOrDefault(p => p.Name == name);
    }

    public Position Create(Position position)
    {
        _scoutingDbContext.Positions.Add(position);
        _scoutingDbContext.SaveChanges();
        return position;
    }

    public Position Update(Position position)
    {
        _scoutingDbContext.Positions.Update(position);
        _scoutingDbContext.SaveChanges();
        return position;
    }

    public bool Delete(int id)
    {
        var position = _scoutingDbContext.Positions.Find(id);
        if (position == null) return false;

        _scoutingDbContext.Positions.Remove(position);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
