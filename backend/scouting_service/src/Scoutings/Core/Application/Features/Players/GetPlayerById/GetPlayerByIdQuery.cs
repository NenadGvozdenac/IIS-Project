using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerById;

public class GetPlayerByIdQuery : IRequest<Result<GetPlayerByIdResponse>>
{
    public int IdPlayer { get; set; }

    public GetPlayerByIdQuery(int idPlayer)
    {
        IdPlayer = idPlayer;
    }
}
