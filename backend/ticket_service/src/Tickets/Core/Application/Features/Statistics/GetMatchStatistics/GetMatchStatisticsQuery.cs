using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Statistics.GetMatchStatistics;

public class GetMatchStatisticsQuery : IRequest<Result<GetMatchStatisticsResponse>>
{
    public int MatchId { get; set; }

    public GetMatchStatisticsQuery(int matchId)
    {
        MatchId = matchId;
    }
}