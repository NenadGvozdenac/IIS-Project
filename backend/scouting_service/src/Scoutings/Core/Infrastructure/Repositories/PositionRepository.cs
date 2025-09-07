using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly ScoutingDbContext _context;

    public PositionRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Position> GetAll()
    {
        return _context.Positions.ToList();
    }

    public Position? GetById(int id)
    {
        return _context.Positions.Find(id);
    }

    public Position Create(Position position)
    {
        _context.Positions.Add(position);
        _context.SaveChanges();
        return position;
    }

    public Position Update(Position position)
    {
        _context.Positions.Update(position);
        _context.SaveChanges();
        return position;
    }

    public void Delete(int id)
    {
        var position = _context.Positions.Find(id);
        if (position != null)
        {
            _context.Positions.Remove(position);
            _context.SaveChanges();
        }
    }
}
