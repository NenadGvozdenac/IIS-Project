using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public SeasonRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public Season? GetById(int id)
    {
        return _scoutingDbContext.Seasons
            .FirstOrDefault(s => s.IdSeason == id);
    }

    public IEnumerable<Season> GetAll()
    {
        return _scoutingDbContext.Seasons
            .ToList();
    }

    public Season? GetByName(string name)
    {
        return _scoutingDbContext.Seasons
            .FirstOrDefault(s => s.Name == name);
    }

    public Season Create(Season season)
    {
        _scoutingDbContext.Seasons.Add(season);
        _scoutingDbContext.SaveChanges();
        return season;
    }

    public Season Update(Season season)
    {
        _scoutingDbContext.Seasons.Update(season);
        _scoutingDbContext.SaveChanges();
        return season;
    }

    public bool Delete(int id)
    {
        var season = _scoutingDbContext.Seasons.Find(id);
        if (season == null) return false;

        _scoutingDbContext.Seasons.Remove(season);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
