namespace ticket_service.src.Tickets.Core.Domain.Entities.Graph;

public abstract class BaseGraphEntity
{
    public string Id { get; set; } = string.Empty;
    public string ElementId { get; set; } = string.Empty;
}