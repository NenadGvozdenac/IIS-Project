using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.DeleteSeason;

public class DeleteSeasonCommand : IRequest<Result<DeleteSeasonResponse>>
{
    public int IdSeason { get; set; }

    public DeleteSeasonCommand(int idSeason)
    {
        IdSeason = idSeason;
    }
}
