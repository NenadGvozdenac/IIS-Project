using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class CompetitionRepository : ICompetitionRepository
{
    private readonly TravelDbContext _travelDbContext;

    public CompetitionRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Competition> GetAll()
    {
        return _travelDbContext.Competitions
            .Include(c => c.Matches)
            .OrderBy(c => c.StartedAt)
            .ToList();
    }
    public Competition? GetById(int id)
    {
        return _travelDbContext.Competitions
            .Include(c => c.Matches)
            .FirstOrDefault(c => c.IdCompetition == id);
    }
}
