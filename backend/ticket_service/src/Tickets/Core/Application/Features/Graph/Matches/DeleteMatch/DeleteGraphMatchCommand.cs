using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.DeleteMatch;

public class DeleteGraphMatchCommand : IRequest<Result<DeleteGraphMatchResponse>>
{
    public string Name { get; set; } = string.Empty;

    public DeleteGraphMatchCommand(string name)
    {
        Name = name;
    }
}