using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.StartMatchOrPeriod;

public class StartMatchOrPeriodCommand : IRequest<Result<StartMatchOrPeriodResponse>>
{
    public int MatchId { get; set; }
}