using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly TravelDbContext _travelDbContext;

    public MemberRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Management> GetAll()
    {
        return _travelDbContext.Managements.ToList();
    }
}
