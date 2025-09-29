using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly TravelDbContext _travelDbContext;

    public MatchRepository(TravelDbContext travelDbContext)
    {
        _travelDbContext = travelDbContext;
    }

    public IEnumerable<Match> GetAll()
    {
        return _travelDbContext.Matches.ToList();
    }

    public Match? GetById(int id)
    {
        return _travelDbContext.Matches.Find(id);
    }

    public Match Create(Match match)
    {
        _travelDbContext.Matches.Add(match);
        _travelDbContext.SaveChanges();
        return match;
    }

    public Match Update(Match match)
    {
        _travelDbContext.Update(match);
        _travelDbContext.SaveChanges();
        return match;
    }

    public bool Delete(int id)
    {
        var match = _travelDbContext.Matches.Find(id);
        if (match == null) return false;

        _travelDbContext.Matches.Remove(match);
        _travelDbContext.SaveChanges();
        return true;
    }
}