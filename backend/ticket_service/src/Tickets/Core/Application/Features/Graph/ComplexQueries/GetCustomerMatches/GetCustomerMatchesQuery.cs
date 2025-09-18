using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetCustomerMatches;

public class GetCustomerMatchesQuery : IRequest<Result<GetCustomerMatchesResponse>>
{
    public GetCustomerMatchesQuery()
    {
    }
}