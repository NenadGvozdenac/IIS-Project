using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetMatchById;

public class GetGraphMatchByIdQuery : IRequest<Result<GetGraphMatchByIdResponse>>
{
    public int Id { get; set; }

    public GetGraphMatchByIdQuery(int id)
    {
        Id = id;
    }
}