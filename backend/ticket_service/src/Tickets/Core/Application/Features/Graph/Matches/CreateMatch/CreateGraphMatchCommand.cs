using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.CreateMatch;

public class CreateGraphMatchCommand : IRequest<Result<CreateGraphMatchResponse>>
{
    public string Name { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public bool IsInOurHall { get; set; }

    public CreateGraphMatchCommand(string name, DateTime scheduledAt, string type, string state, string city, string hall, bool isInOurHall)
    {
        Name = name;
        ScheduledAt = scheduledAt;
        Type = type;
        State = state;
        City = city;
        Hall = hall;
        IsInOurHall = isInOurHall;
    }
}