using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetTicketByName;

public class GetGraphIndividualTicketByNameQuery : IRequest<Result<GetGraphIndividualTicketByNameResponse>>
{
    public string Name { get; set; } = string.Empty;

    public GetGraphIndividualTicketByNameQuery(string name)
    {
        Name = name;
    }
}