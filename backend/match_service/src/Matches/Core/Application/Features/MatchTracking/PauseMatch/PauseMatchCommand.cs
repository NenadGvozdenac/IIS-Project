using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.PauseMatch;

public class PauseMatchCommand : IRequest<Result<PauseMatchResponse>>
{
    public int MatchId { get; set; }
}