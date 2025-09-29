using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetTeamStatistics
{
    public class GetTeamStatisticsQuery : IRequest<Result<GetTeamStatisticsResponse>>
    {
        public int TeamId { get; set; }
    }
}