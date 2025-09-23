using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.EndMatch;

public class EndMatchCommand : IRequest<Result<EndMatchResponse>>
{
    public int MatchId { get; set; }
}