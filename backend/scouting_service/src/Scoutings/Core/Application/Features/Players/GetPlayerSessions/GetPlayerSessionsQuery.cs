using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSessions;

public class GetPlayerSessionsQuery : IRequest<Result<List<GetPlayerSessionsResponse>>>
{
    public int PlayerId { get; set; }
    public int? SeasonId { get; set; }
    public string? Status { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }

    public GetPlayerSessionsQuery(int playerId, int? seasonId, string? status, string? dateFrom, string? dateTo)
    {
        PlayerId = playerId;
        SeasonId = seasonId;
        Status = status;
        DateFrom = dateFrom;
        DateTo = dateTo;
    }
}