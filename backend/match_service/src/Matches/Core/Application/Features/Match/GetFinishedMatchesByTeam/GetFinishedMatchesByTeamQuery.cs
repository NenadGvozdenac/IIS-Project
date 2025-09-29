using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Match.GetFinishedMatchesByTeam;

public class GetFinishedMatchesByTeamQuery : IRequest<Result<GetFinishedMatchesByTeamResponse>>
{
    public int TeamId { get; set; }
}