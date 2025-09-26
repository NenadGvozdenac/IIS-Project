using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSeasonMetricAverages;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSessions;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerRecommendations;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IPlayerRepository
{
    Player? GetById(int id);
    Player Create(Player player);
    Player Update(Player player);
    void Delete(int id);
    IEnumerable<Player> GetAll();
    IEnumerable<Player> GetByNationality(int nationalityId);
    IEnumerable<Player> GetByPosition(int positionId);
    Task<List<GetPlayerSeasonMetricAveragesResponse>> GetPlayerSeasonMetricAveragesAsync(int playerId, int seasonId, string? sessionType);
    Task<List<GetPlayerSessionsResponse>> GetPlayerSessionsAsync(int playerId, int? seasonId, string? status, string? dateFrom, string? dateTo);
    Task<List<PlayerRecommendation>> GetPlayerRecommendationsAsync(List<MetricWeight> metricWeights);
}
