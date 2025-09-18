using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.CreateTicket;

public class CreateGraphIndividualTicketHandler : IRequestHandler<CreateGraphIndividualTicketCommand, Result<CreateGraphIndividualTicketResponse>>
{
    private readonly IGraphIndividualTicketRepository _ticketRepository;

    public CreateGraphIndividualTicketHandler(IGraphIndividualTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<CreateGraphIndividualTicketResponse>> Handle(CreateGraphIndividualTicketCommand request, CancellationToken cancellationToken)
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

            ticket = await _ticketRepository.CreateIndividualTicket(ticket);

            if(ticket == null)
            {
                return Result<CreateGraphIndividualTicketResponse>.Failure("Failed to create ticket");
            }

            var response = new CreateGraphIndividualTicketResponse(ticket.Id, ticket.ElementId, ticket.Name, ticket.Description, ticket.Type, ticket.ReleasedAt, ticket.Price);
            return Result<CreateGraphIndividualTicketResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateGraphIndividualTicketResponse>.Failure($"Failed to create ticket: {ex.Message}");
        }
    }
}