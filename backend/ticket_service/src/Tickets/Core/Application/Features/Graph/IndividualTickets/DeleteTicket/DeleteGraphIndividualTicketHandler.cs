using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.DeleteTicket;

public class DeleteGraphIndividualTicketHandler : IRequestHandler<DeleteGraphIndividualTicketCommand, Result<DeleteGraphIndividualTicketResponse>>
{
    private readonly IGraphIndividualTicketRepository _ticketRepository;

    public DeleteGraphIndividualTicketHandler(IGraphIndividualTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<DeleteGraphIndividualTicketResponse>> Handle(DeleteGraphIndividualTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _ticketRepository.DeleteIndividualTicket(request.Id);
            var response = new DeleteGraphIndividualTicketResponse(request.Id, "Ticket deleted successfully");
            return Result<DeleteGraphIndividualTicketResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<DeleteGraphIndividualTicketResponse>.Failure($"Failed to delete ticket: {ex.Message}");
        }
    }
}