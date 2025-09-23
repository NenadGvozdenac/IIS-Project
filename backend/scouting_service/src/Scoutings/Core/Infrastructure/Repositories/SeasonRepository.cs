using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly ScoutingDbContext _context;

    public SeasonRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Season> GetAll()
    {
        return _context.Seasons.ToList();
    }

    public Season? GetById(int id)
    {
        return _context.Seasons.Find(id);
    }

    public Season Create(Season season)
    {
        _context.Seasons.Add(season);
        _context.SaveChanges();
        return season;
    }

    public Season Update(Season season)
    {
        _context.Seasons.Update(season);
        _context.SaveChanges();
        return season;
    }

    public void Delete(int id)
    {
        var season = _context.Seasons.Find(id);
        if (season != null)
        {
            _context.Seasons.Remove(season);
            _context.SaveChanges();
        }
    }
}
