namespace travel_service.src.Travels.Core.Application.Features.Agencies.GetAllAgencies;

public class GetAllAgenciesResponse
{
    public List<AgencyDto> Agencies { get; set; } = new List<AgencyDto>();
}

public class AgencyDto
{
    public int IdAgency { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Type { get; set; }
}

