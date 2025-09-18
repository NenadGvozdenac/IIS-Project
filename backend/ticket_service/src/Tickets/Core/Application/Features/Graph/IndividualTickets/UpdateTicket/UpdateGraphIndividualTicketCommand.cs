using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.UpdateTicket;

public class UpdateGraphIndividualTicketCommand : IRequest<Result<UpdateGraphIndividualTicketResponse>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
    public decimal Price { get; set; }

    public UpdateGraphIndividualTicketCommand(int id, string name, string description, string type, DateTime releasedAt, decimal price)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        ReleasedAt = releasedAt;
        Price = price;
    }
}