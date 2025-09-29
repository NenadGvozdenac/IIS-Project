using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSeasonMetricAverages;

public class GetPlayerSeasonMetricAveragesQuery : IRequest<Result<List<GetPlayerSeasonMetricAveragesResponse>>>
{
    public int PlayerId { get; set; }
    public int SeasonId { get; set; }
    public string? SessionType { get; set; }

    public GetPlayerSeasonMetricAveragesQuery(int playerId, int seasonId, string? sessionType)
    {
        PlayerId = playerId;
        SeasonId = seasonId;
        SessionType = sessionType;
    }
}