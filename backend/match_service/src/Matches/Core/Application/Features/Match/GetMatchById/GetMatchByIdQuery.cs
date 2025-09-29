using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Match.GetMatchById;

public class GetMatchByIdQuery : IRequest<Result<GetMatchByIdResponse>>
{
    public int Id { get; set; }

    public GetMatchByIdQuery(int id)
    {
        Id = id;
    }
}
