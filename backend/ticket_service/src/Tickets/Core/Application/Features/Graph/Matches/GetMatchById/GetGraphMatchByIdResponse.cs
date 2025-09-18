namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetMatchById;

public class GetGraphMatchByIdResponse
{
    public int Id { get; set; }
    public string ElementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public bool IsInOurHall { get; set; }

    public GetGraphMatchByIdResponse(int id, string elementId, string name, DateTime scheduledAt, string type, string state, string city, string hall, bool isInOurHall)
    {
        Id = id;
        ElementId = elementId;
        Name = name;
        ScheduledAt = scheduledAt;
        Type = type;
        State = state;
        City = city;
        Hall = hall;
        IsInOurHall = isInOurHall;
    }
}