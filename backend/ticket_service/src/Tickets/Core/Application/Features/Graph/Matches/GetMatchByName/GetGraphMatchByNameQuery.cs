using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetMatchByName;

public class GetGraphMatchByNameQuery : IRequest<Result<GetGraphMatchByNameResponse>>
{
    public string Name { get; set; } = string.Empty;

    public GetGraphMatchByNameQuery(string name)
    {
        Name = name;
    }
}