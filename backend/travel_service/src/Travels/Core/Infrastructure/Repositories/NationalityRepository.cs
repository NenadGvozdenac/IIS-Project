using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class NationalityRepository : INationalityRepository
{
    private readonly TravelDbContext _travelDbContext;

    public NationalityRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Nationality> GetAll()
    {
        return _travelDbContext.Nationalities.ToList();
    }
    public Nationality? GetById(int id)
    {
        return _travelDbContext.Nationalities
            .FirstOrDefault(n => n.IdNationality == id);
    }
}