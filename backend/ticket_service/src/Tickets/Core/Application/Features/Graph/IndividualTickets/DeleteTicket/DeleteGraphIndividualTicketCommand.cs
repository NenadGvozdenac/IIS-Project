using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.DeleteTicket;

public class DeleteGraphIndividualTicketCommand : IRequest<Result<DeleteGraphIndividualTicketResponse>>
{
    public string Name { get; set; } = string.Empty;

    public DeleteGraphIndividualTicketCommand(string name)
    {
        Name = name;
    }
}