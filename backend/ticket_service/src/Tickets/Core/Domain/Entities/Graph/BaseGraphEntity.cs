namespace ticket_service.src.Tickets.Core.Domain.Entities.Graph;

public abstract class BaseGraphEntity
{
    public int Id { get; set; }
    public string ElementId { get; set; } = string.Empty;
}