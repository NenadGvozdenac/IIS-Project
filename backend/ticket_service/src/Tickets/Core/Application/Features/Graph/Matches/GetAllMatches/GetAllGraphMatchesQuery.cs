using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetAllMatches;

public class GetAllGraphMatchesQuery : IRequest<Result<List<GetAllGraphMatchesResponse>>>
{
}