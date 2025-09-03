using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly TravelDbContext _travelDbContext;

    public SeasonRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Season> GetAll()
    {
        return _travelDbContext.Seasons.ToList();
    }
}
