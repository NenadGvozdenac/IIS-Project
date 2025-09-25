namespace travel_service.src.Travels.Core.Application.Features.Trip.GetTravelCostReport;

public class TravelCostReportDto
{
    public List<MatchCostDto> IndividualCosts { get; set; } = new();
    public TotalCostsDto TotalCosts { get; set; } = new();
}

public class MatchCostDto
{
    public MatchInfoDto Match { get; set; } = new();
    public CostDetailsDto Transportation { get; set; } = new();
    public CostDetailsDto Accommodation { get; set; } = new();
    public decimal TotalCost { get; set; }
    public TravelersDto Travelers { get; set; } = new();
}

public class MatchInfoDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public string OpponentTeam { get; set; } = string.Empty;
}

public class CostDetailsDto
{
    public decimal Cost { get; set; }
    public OfferDetailsDto? Details { get; set; }
}

public class OfferDetailsDto
{
    public string? Company { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
}

public class TravelersDto
{
    public List<string> TeamMembers { get; set; } = new();
    public List<string> Management { get; set; } = new();
}

public class TotalCostsDto
{
    public decimal Transportation { get; set; }
    public decimal Accommodation { get; set; }
    public decimal Total { get; set; }
    public int MatchCount { get; set; }
}