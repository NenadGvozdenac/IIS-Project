using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetAllIndividualTickets;

public class GetAllGraphIndividualTicketsQuery : IRequest<Result<List<GetAllGraphIndividualTicketsResponse>>>
{
}