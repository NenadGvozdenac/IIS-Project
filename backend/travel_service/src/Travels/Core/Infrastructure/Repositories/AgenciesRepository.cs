using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;
using travel_service.src.Travels.Core.Application.Features.Agencies.GetAgenciesForMatchAndType;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class AgenciesRepository : IAgenciesRepository
{
    private readonly TravelDbContext _travelDbContext;

    public AgenciesRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Agency> GetAll()
    {
        return _travelDbContext.Agencies.ToList();
    }

    public IEnumerable<Agency> GetByType(string type)
    {
        return _travelDbContext.Agencies
            .Where(a => a.Type == type)
            .ToList();
    }

    public List<AgencyWithRequestInfo> GetAgenciesForMatchAndType(int matchId, string type)
    {
        // Join sent_request with agencies and requests to get agencies that sent requests for this match and type
        var result = (from sr in _travelDbContext.SentRequests
                     join a in _travelDbContext.Agencies on sr.IdAgency equals a.IdAgency
                     join r in _travelDbContext.Requests on sr.IdRequest equals r.IdRequest
                     where r.IdMatch == matchId && r.Type == type
                     select new AgencyWithRequestInfo
                     {
                         IdAgency = a.IdAgency,
                         AgencyName = a.Name ?? string.Empty,
                         IdRequest = r.IdRequest,
                         RequestType = r.Type ?? string.Empty
                     }).ToList();

        return result;
    }
}
