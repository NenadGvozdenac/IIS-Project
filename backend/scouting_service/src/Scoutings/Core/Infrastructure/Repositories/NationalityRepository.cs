using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class NationalityRepository : INationalityRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public NationalityRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public Nationality? GetById(int id)
    {
        return _scoutingDbContext.Nationalities
            .Include(n => n.Players)
            .FirstOrDefault(n => n.IdNationality == id);
    }

    public IEnumerable<Nationality> GetAll()
    {
        return _scoutingDbContext.Nationalities
            .Include(n => n.Players)
            .ToList();
    }

    public Nationality? GetByName(string name)
    {
        return _scoutingDbContext.Nationalities
            .Include(n => n.Players)
            .FirstOrDefault(n => n.Name == name);
    }

    public Nationality Create(Nationality nationality)
    {
        _scoutingDbContext.Nationalities.Add(nationality);
        _scoutingDbContext.SaveChanges();
        return nationality;
    }

    public Nationality Update(Nationality nationality)
    {
        _scoutingDbContext.Nationalities.Update(nationality);
        _scoutingDbContext.SaveChanges();
        return nationality;
    }

    public bool Delete(int id)
    {
        var nationality = _scoutingDbContext.Nationalities.Find(id);
        if (nationality == null) return false;

        _scoutingDbContext.Nationalities.Remove(nationality);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
