using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly TravelDbContext _travelDbContext;

    public TeamMemberRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<TeamMember> GetAll()
    {
        return _travelDbContext.TeamMembers.ToList();
    }
}
