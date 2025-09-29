using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetTicketById;

public class GetGraphIndividualTicketByIdHandler : IRequestHandler<GetGraphIndividualTicketByIdQuery, Result<GetGraphIndividualTicketByIdResponse>>
{
    private readonly IGraphIndividualTicketRepository _ticketRepository;

    public GetGraphIndividualTicketByIdHandler(IGraphIndividualTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<GetGraphIndividualTicketByIdResponse>> Handle(GetGraphIndividualTicketByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _ticketRepository.GetIndividualTicketById(request.Id);
            if (ticket == null)
            {
                return Result<GetGraphIndividualTicketByIdResponse>.Failure("Ticket not found");
            }

            var response = new GetGraphIndividualTicketByIdResponse(ticket.Id, ticket.ElementId, ticket.Name, ticket.Description, ticket.Type, ticket.ReleasedAt, ticket.Price);
            return Result<GetGraphIndividualTicketByIdResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetGraphIndividualTicketByIdResponse>.Failure($"Failed to get ticket by id: {ex.Message}");
        }
    }
}