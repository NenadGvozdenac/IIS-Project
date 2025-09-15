using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.PlayerSubstitution;

public class PlayerSubstitutionCommand : IRequest<Result<PlayerSubstitutionResponse>>
{
    public int MatchId { get; set; }
    public int TeamId { get; set; }
    public int PlayerInId { get; set; }
    public int PlayerOutId { get; set; }
}