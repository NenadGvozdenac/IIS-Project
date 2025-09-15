using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.Timeout;

public class TimeoutCommand : IRequest<Result<TimeoutResponse>>
{
    public int MatchId { get; set; }
    public int TeamId { get; set; }
}