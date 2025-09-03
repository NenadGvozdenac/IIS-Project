using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.GetTeamPlayersByTeamId;

public class GetTeamPlayersByTeamIdQuery : IRequest<Result<GetTeamPlayersByTeamIdResponse>>
{
    public int TeamId { get; set; }

    public GetTeamPlayersByTeamIdQuery(int teamId)
    {
        TeamId = teamId;
    }
}
