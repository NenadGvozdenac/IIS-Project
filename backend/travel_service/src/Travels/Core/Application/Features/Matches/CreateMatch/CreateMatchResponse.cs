namespace travel_service.src.Travels.Core.Application.Features.Matches.CreateMatch;

public class CreateMatchResponse
{
    public int IdMatch { get; set; }
    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Type { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Hall { get; set; } = null!;

    public bool IsInOurHall { get; set; }

    public bool TransportationRequired { get; set; }

    public bool AccommodationRequired { get; set; }

    public int IdSeason { get; set; }

    public int IdTeam { get; set; }
}