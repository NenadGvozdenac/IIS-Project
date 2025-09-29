using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class VisaRepository : IVisaRepository
{
    private readonly TravelDbContext _travelDbContext;

    public VisaRepository(TravelDbContext travelDbContext)
    {
        _travelDbContext = travelDbContext;
    }

    public Visa Create(Visa visa)
    {
        _travelDbContext.Visas.Add(visa);
        _travelDbContext.SaveChanges();
        return visa;
    }

    public IEnumerable<Visa> GetByTravelInformationId(int travelInfoId)
    {
        return _travelDbContext.Visas
            .Where(v => v.IdTravelInformation == travelInfoId)
            .ToList();
    }

    // Saga deletion methods
    public IEnumerable<Visa> GetByTravelInformationIds(IEnumerable<int> travelInfoIds)
    {
        return _travelDbContext.Visas
            .Where(v => travelInfoIds.Contains(v.IdTravelInformation))
            .ToList();
    }

    public bool Delete(string visaNumber)
    {
        var visa = _travelDbContext.Visas.Find(visaNumber);
        if (visa == null)
            return false;

        _travelDbContext.Visas.Remove(visa);
        _travelDbContext.SaveChanges();
        return true;
    }
}