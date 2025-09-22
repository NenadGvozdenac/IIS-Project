using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Match.GetMatchPlayerStatistics;

public class GetMatchPlayerStatisticsQuery : IRequest<Result<GetMatchPlayerStatisticsResponse>>
{
    public int MatchId { get; set; }

    public GetMatchPlayerStatisticsQuery(int matchId)
    {
        MatchId = matchId;
    }
}