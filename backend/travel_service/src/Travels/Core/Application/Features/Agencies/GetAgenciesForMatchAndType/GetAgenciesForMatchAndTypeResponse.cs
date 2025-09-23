using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.Agencies.GetAgenciesForMatchAndType;

public class GetAgenciesForMatchAndTypeResponse
{
    public List<AgencyWithRequestInfo> Agencies { get; set; } = new List<AgencyWithRequestInfo>();
}

public class AgencyWithRequestInfo
{
    public int IdAgency { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public int IdRequest { get; set; }
    public string RequestType { get; set; } = string.Empty;
}