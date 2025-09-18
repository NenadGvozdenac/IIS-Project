using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seasons.GetSeasonById;

public class GetSeasonByIdQuery : IRequest<Result<GetSeasonByIdResponse>>
{
    public int IdSeason { get; set; }

    public GetSeasonByIdQuery(int idSeason)
    {
        IdSeason = idSeason;
    }
}
