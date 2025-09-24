using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetAdvancedMatchStatistics
{
    public class GetAdvancedMatchStatisticsQuery : IRequest<Result<GetAdvancedMatchStatisticsResponse>>
    {
        public string MatchId { get; set; } = string.Empty;
    }
}