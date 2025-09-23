using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class NationalityRepository : INationalityRepository
{
    private readonly ScoutingDbContext _context;

    public NationalityRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Nationality> GetAll()
    {
        return _context.Nationalities.ToList();
    }

    public Nationality? GetById(int id)
    {
        return _context.Nationalities.Find(id);
    }

    public Nationality Create(Nationality nationality)
    {
        _context.Nationalities.Add(nationality);
        _context.SaveChanges();
        return nationality;
    }

    public Nationality Update(Nationality nationality)
    {
        _context.Nationalities.Update(nationality);
        _context.SaveChanges();
        return nationality;
    }

    public void Delete(int id)
    {
        var nationality = _context.Nationalities.Find(id);
        if (nationality != null)
        {
            _context.Nationalities.Remove(nationality);
            _context.SaveChanges();
        }
    }
}
