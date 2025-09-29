using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface ITravelInfoRepository
{
    TravelInformation? GetById(int id);
    IEnumerable<TravelInformation> GetAll();
    TravelInformation Create(TravelInformation travelInformation);
    TravelInformation Update(TravelInformation travelInformation);
    
    // Saga deletion methods
    IEnumerable<TravelInformation> GetByPlayerAndTeam(int playerId, int teamId);
    bool Delete(int id);
}