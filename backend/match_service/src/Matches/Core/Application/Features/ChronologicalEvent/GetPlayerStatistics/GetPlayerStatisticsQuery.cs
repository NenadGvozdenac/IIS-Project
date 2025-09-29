using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerStatistics;

public class GetPlayerStatisticsQuery : IRequest<Result<GetPlayerStatisticsResponse>>
{
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    
    public GetPlayerStatisticsQuery(int playerId, int teamId)
    {
        PlayerId = playerId;
        TeamId = teamId;
    }
}