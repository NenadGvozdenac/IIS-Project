using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetTicketByName;

public class GetGraphIndividualTicketByNameHandler : IRequestHandler<GetGraphIndividualTicketByNameQuery, Result<GetGraphIndividualTicketByNameResponse>>
{
    private readonly IGraphIndividualTicketRepository _ticketRepository;

    public GetGraphIndividualTicketByNameHandler(IGraphIndividualTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<GetGraphIndividualTicketByNameResponse>> Handle(GetGraphIndividualTicketByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _ticketRepository.GetIndividualTicketByName(request.Name);
            if (ticket == null)
            {
                return Result<GetGraphIndividualTicketByNameResponse>.Failure("Ticket not found");
            }

            var response = new GetGraphIndividualTicketByNameResponse(ticket.Name, ticket.Description, ticket.Type, ticket.ReleasedAt, ticket.Price);
            return Result<GetGraphIndividualTicketByNameResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetGraphIndividualTicketByNameResponse>.Failure($"Failed to get ticket: {ex.Message}");
        }
    }
}