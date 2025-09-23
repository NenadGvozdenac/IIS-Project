using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.GetSessionById;

public class GetSessionByIdQuery : IRequest<Result<GetSessionByIdResponse>>
{
    public int Id { get; set; }

    public GetSessionByIdQuery(int id)
    {
        Id = id;
    }
}
