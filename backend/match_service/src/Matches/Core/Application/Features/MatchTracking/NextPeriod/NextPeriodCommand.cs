using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.NextPeriod;

public class NextPeriodCommand : IRequest<Result<NextPeriodResponse>>
{
    public int MatchId { get; set; }
}