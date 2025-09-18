using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.UpdateTicket;

public class UpdateGraphIndividualTicketHandler : IRequestHandler<UpdateGraphIndividualTicketCommand, Result<UpdateGraphIndividualTicketResponse>>
{
    private readonly IGraphIndividualTicketRepository _ticketRepository;

    public UpdateGraphIndividualTicketHandler(IGraphIndividualTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<UpdateGraphIndividualTicketResponse>> Handle(UpdateGraphIndividualTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = new IndividualTicket
            {
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                ReleasedAt = request.ReleasedAt,
                Price = request.Price
            };

            await _ticketRepository.UpdateIndividualTicket(request.Id, ticket);
            var response = new UpdateGraphIndividualTicketResponse(ticket.Name, ticket.Description, ticket.Type, ticket.ReleasedAt, ticket.Price);
            return Result<UpdateGraphIndividualTicketResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<UpdateGraphIndividualTicketResponse>.Failure($"Failed to update ticket: {ex.Message}");
        }
    }
}