using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Application.Features.Agencies.GetAgenciesForMatchAndType;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IAgenciesRepository
{
    IEnumerable<Agency> GetAll();
    IEnumerable<Agency> GetByType(string type);
    List<AgencyWithRequestInfo> GetAgenciesForMatchAndType(int matchId, string type);
}