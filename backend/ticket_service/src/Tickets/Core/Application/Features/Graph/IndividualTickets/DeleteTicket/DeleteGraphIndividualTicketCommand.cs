using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.DeleteTicket;

public class DeleteGraphIndividualTicketCommand : IRequest<Result<DeleteGraphIndividualTicketResponse>>
{
    public int Id { get; set; }

    public DeleteGraphIndividualTicketCommand(int id)
    {
        Id = id;
    }
}