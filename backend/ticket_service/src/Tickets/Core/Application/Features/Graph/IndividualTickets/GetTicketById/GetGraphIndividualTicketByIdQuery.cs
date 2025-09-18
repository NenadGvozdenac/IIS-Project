using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetTicketById;

public class GetGraphIndividualTicketByIdQuery : IRequest<Result<GetGraphIndividualTicketByIdResponse>>
{
    public int Id { get; set; }

    public GetGraphIndividualTicketByIdQuery(int id)
    {
        Id = id;
    }
}