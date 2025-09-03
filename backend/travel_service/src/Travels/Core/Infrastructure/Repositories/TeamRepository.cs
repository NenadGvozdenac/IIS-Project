using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly TravelDbContext _travelDbContext;

    public TeamRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Team> GetAll()
    {
        return _travelDbContext.Teams.ToList();
    }
}