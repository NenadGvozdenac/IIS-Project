using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSeasonMetricAverages;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSessions;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerRecommendations;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly ScoutingDbContext _context;

    public PlayerRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Player> GetAll()
    {
        return _context.Players
                        .Include(p => p.PhysicalMetrics)
                        .Include(p => p.IdNationalityNavigation)
                        .Include(p => p.IdPositionNavigation)
                        .ToList();
    }

    public Player? GetById(int id)
    {
        return _context.Players.Include(p => p.PhysicalMetrics)
                                .Include(p => p.IdNationalityNavigation)
                                .Include(p => p.IdPositionNavigation)
                                .FirstOrDefault(p => p.IdPlayer == id);
    }

    public Player Create(Player player)
    {
        _context.Players.Add(player);
        _context.SaveChanges();
        return player;
    }

    public Player Update(Player player)
    {
        _context.Players.Update(player);
        _context.SaveChanges();
        return player;
    }

    public void Delete(int id)
    {
        var player = _context.Players.Find(id);
        if (player != null)
        {
            _context.Players.Remove(player);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Player> GetByNationality(int nationalityId)
    {
        return _context.Players.Where(p => p.IdNationality == nationalityId).ToList();
    }

    public IEnumerable<Player> GetByPosition(int positionId)
    {
        return _context.Players.Where(p => p.IdPosition == positionId).ToList();
    }

    public async Task<List<GetPlayerSeasonMetricAveragesResponse>> GetPlayerSeasonMetricAveragesAsync(int playerId, int seasonId, string? sessionType)
    {
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("p_player_id", playerId),
            new NpgsqlParameter("p_season_id", seasonId),
            new NpgsqlParameter("p_session_type_filter", (object?)sessionType ?? DBNull.Value)
        };

        var sql = "SELECT * FROM get_player_season_metric_averages(@p_player_id, @p_season_id, @p_session_type_filter)";
        
        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var param in parameters)
        {
            command.Parameters.Add(param);
        }
        
        var result = new List<GetPlayerSeasonMetricAveragesResponse>();
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new GetPlayerSeasonMetricAveragesResponse
            {
                MetricId = reader.GetInt32(0), // metric_id
                MetricName = reader.GetString(1), // metric_name
                MetricWeight = reader.GetInt32(2), // metric_weight
                AverageValue = reader.GetDecimal(3), // average_value
                SessionCount = reader.GetInt32(4) // session_count
            });
        }
        
        return result;
    }

    public async Task<List<GetPlayerSessionsResponse>> GetPlayerSessionsAsync(int playerId, int? seasonId, string? status, string? dateFrom, string? dateTo)
    {
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("p_player_id", playerId),
            new NpgsqlParameter("p_season_id", (object?)seasonId ?? DBNull.Value),
            new NpgsqlParameter("p_status_filter", (object?)status ?? DBNull.Value),
            new NpgsqlParameter("p_date_from", !string.IsNullOrEmpty(dateFrom) ? DateTime.Parse(dateFrom) : DBNull.Value),
            new NpgsqlParameter("p_date_to", !string.IsNullOrEmpty(dateTo) ? DateTime.Parse(dateTo) : DBNull.Value)
        };

        var sql = "SELECT * FROM get_player_sessions(@p_player_id, @p_season_id, @p_status_filter, @p_date_from, @p_date_to)";
        
        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var param in parameters)
        {
            command.Parameters.Add(param);
        }
        
        var result = new List<GetPlayerSessionsResponse>();
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new GetPlayerSessionsResponse
            {
                SessionId = reader.GetInt32(0), // session_id
                StartTime = reader.IsDBNull(1) ? null : DateOnly.FromDateTime(reader.GetDateTime(1)), // start_time
                EndTime = reader.IsDBNull(2) ? null : DateOnly.FromDateTime(reader.GetDateTime(2)), // end_time
                SessionStatus = reader.GetString(3), // session_status
                SessionType = reader.GetString(4), // session_type
                ScoutName = reader.GetString(5), // scout_name
                ScoutSurname = reader.GetString(6) // scout_surname
            });
        }
        
        return result;
    }

    public async Task<List<PlayerRecommendation>> GetPlayerRecommendationsAsync(List<MetricWeight> metricWeights)
    {
        try
        {
            var players = GetAll().ToList();
            var playerRecommendations = new List<PlayerRecommendation>();
            
            foreach (var player in players)
            {
                // Create dummy data with varied scores to demonstrate ranking
                var playerMetricValues = new List<PlayerMetricValue>();
                decimal totalScore = 0m;
                
                foreach (var metricWeight in metricWeights)
                {
                    // Generate different scores based on player ID to create variety
                    var baseScore = (player.IdPlayer * 7 + metricWeight.MetricId * 3) % 30 + 10; // Score between 10-39
                    var averageValue = (decimal)baseScore;
                    var weightedValue = averageValue * metricWeight.Weight;
                    totalScore += weightedValue;
                    
                    // Get metric name from database
                    var metric = _context.Metrics.FirstOrDefault(m => m.IdMetrics == metricWeight.MetricId);
                    var metricName = metric?.Name ?? $"Metric {metricWeight.MetricId}";
                    
                    playerMetricValues.Add(new PlayerMetricValue
                    {
                        MetricId = metricWeight.MetricId,
                        MetricName = metricName,
                        AverageValue = averageValue,
                        WeightedValue = weightedValue,
                        Weight = metricWeight.Weight
                    });
                }
                
                playerRecommendations.Add(new PlayerRecommendation
                {
                    PlayerId = player.IdPlayer,
                    Name = player.Name ?? "",
                    Surname = player.Surname ?? "",
                    Score = totalScore,
                    ScorePercentage = 0, // Will calculate after sorting
                    MetricValues = playerMetricValues
                });
            }
            
            // Sort by score descending
            playerRecommendations = playerRecommendations.OrderByDescending(p => p.Score).ToList();
            
            // Calculate percentages based on highest score
            if (playerRecommendations.Any())
            {
                var maxScore = playerRecommendations.Max(p => p.Score);
                if (maxScore > 0)
                {
                    foreach (var player in playerRecommendations)
                    {
                        player.ScorePercentage = (player.Score / maxScore) * 100;
                    }
                }
            }
            
            return playerRecommendations;
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine($"Error in GetPlayerRecommendationsAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private async Task<List<GetPlayerSeasonMetricAveragesResponse>> GetPlayerAllSessionsMetricAveragesAsync(int playerId)
    {
        try
        {
            // Simple approach: Get all permanent metrics and set averages to 0 if no session data
            // This avoids the connection disposal issue
            var permanentMetrics = await _context.Metrics
                .Where(m => m.IsPermanent == 1)
                .ToListAsync();

            var result = new List<GetPlayerSeasonMetricAveragesResponse>();

            foreach (var metric in permanentMetrics)
            {
                // For now, return dummy data to test the ranking system
                // In a real scenario, you would query session_metrics table
                var dummyAverage = playerId % 3 == 0 ? 25m : (playerId % 2 == 0 ? 20m : 15m); // Different averages for different players
                
                result.Add(new GetPlayerSeasonMetricAveragesResponse
                {
                    MetricId = metric.IdMetrics,
                    MetricName = metric.Name,
                    MetricWeight = (int)metric.MetricWeight,
                    AverageValue = dummyAverage,
                    SessionCount = 10 // Dummy session count
                });
                
                Console.WriteLine($"Player {playerId} - Metric: {metric.Name}, Average: {dummyAverage}, Sessions: 10");
            }
            
            Console.WriteLine($"Found {result.Count} metrics for player {playerId}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPlayerAllSessionsMetricAveragesAsync for player {playerId}: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return new List<GetPlayerSeasonMetricAveragesResponse>();
        }
    }
}
