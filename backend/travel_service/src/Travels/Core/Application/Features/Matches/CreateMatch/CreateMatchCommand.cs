using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Matches.CreateMatch;

public class CreateMatchCommand : IRequest<Result<CreateMatchResponse>>
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }

    public string Type { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Hall { get; set; } = null!;

    public bool IsInOurHall { get; set; }

    public bool TransportationRequired { get; set; }

    public bool AccommodationRequired { get; set; }

    public int SeasonId { get; set; }

    public int TeamId { get; set; }
}