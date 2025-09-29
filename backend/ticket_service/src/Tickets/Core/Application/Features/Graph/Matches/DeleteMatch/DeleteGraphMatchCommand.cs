using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.DeleteMatch;

public class DeleteGraphMatchCommand : IRequest<Result<DeleteGraphMatchResponse>>
{
    public int Id { get; set; }

    public DeleteGraphMatchCommand(int id)
    {
        Id = id;
    }
}