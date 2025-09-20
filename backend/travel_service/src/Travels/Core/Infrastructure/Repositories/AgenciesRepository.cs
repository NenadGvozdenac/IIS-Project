using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class AgenciesRepository : IAgenciesRepository
{
    private readonly TravelDbContext _travelDbContext;

    public AgenciesRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Agency> GetAll()
    {
        return _travelDbContext.Agencies.ToList();
    }

    public IEnumerable<Agency> GetByType(string type)
    {
        return _travelDbContext.Agencies
            .Where(a => a.Type == type)
            .ToList();
    }
}
