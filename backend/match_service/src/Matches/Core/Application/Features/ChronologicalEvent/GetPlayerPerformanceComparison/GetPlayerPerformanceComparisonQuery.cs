using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerPerformanceComparison
{
    public class GetPlayerPerformanceComparisonQuery : IRequest<Result<GetPlayerPerformanceComparisonResponse>>
    {
        public string MatchId { get; set; } = string.Empty;
    }
}