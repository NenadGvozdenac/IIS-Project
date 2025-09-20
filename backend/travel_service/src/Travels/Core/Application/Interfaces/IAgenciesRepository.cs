using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IAgenciesRepository
{
    IEnumerable<Agency> GetAll();
    IEnumerable<Agency> GetByType(string type);
}