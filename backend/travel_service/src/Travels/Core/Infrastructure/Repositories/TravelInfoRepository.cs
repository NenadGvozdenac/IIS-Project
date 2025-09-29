using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class TravelInfoRepository : ITravelInfoRepository
{
    private readonly TravelDbContext _travelDbContext;

    public TravelInfoRepository(TravelDbContext travelDbContext)
    {
        _travelDbContext = travelDbContext;
    }

    public IEnumerable<TravelInformation> GetAll()
    {
        return _travelDbContext.TravelInformations.ToList();
    }

    public TravelInformation? GetById(int id)
    {
        return _travelDbContext.TravelInformations.Find(id);
    }

    public TravelInformation Create(TravelInformation travelInformation)
    {
        _travelDbContext.TravelInformations.Add(travelInformation);
        _travelDbContext.SaveChanges();
        return travelInformation;
    }

    public TravelInformation Update(TravelInformation travelInformation)
    {
        _travelDbContext.Update(travelInformation);
        _travelDbContext.SaveChanges();
        return travelInformation;
    }

    // Saga deletion methods
    public IEnumerable<TravelInformation> GetByPlayerAndTeam(int playerId, int teamId)
    {
        return _travelDbContext.TravelInformations
            .Where(ti => ti.IdPlayer == playerId && ti.IdTeam == teamId)
            .ToList();
    }

    public bool Delete(int id)
    {
        var travelInfo = GetById(id);
        if (travelInfo == null)
            return false;

        _travelDbContext.TravelInformations.Remove(travelInfo);
        _travelDbContext.SaveChanges();
        return true;
    }
}