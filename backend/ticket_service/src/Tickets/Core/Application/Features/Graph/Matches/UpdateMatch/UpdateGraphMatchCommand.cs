using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.UpdateMatch;

public class UpdateGraphMatchCommand : IRequest<Result<UpdateGraphMatchResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string NewName { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public bool IsInOurHall { get; set; }

    public UpdateGraphMatchCommand(string name, string newName, DateTime scheduledAt, string type, string state, string city, string hall, bool isInOurHall)
    {
        Name = name;
        NewName = newName;
        ScheduledAt = scheduledAt;
        Type = type;
        State = state;
        City = city;
        Hall = hall;
        IsInOurHall = isInOurHall;
    }
}