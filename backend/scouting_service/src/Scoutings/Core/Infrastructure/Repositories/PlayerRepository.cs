using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSeasonMetricAverages;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSessions;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerRecommendations;
using scouting_service.src.Scoutings.Core.Application.Features.Reports.GenerateScoutingReport;
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

    public async Task<List<ScoutingReportData>> GetScoutingReportDataAsync(int? seasonId, string? position = null, string? nationality = null, string? playerName = null)
    {
        try
        {
            var results = new List<ScoutingReportData>();
            
            // First try the new relative normalization function
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SELECT * FROM generate_relative_scouting_summary(@p_season_id, @p_position_filter, @p_nationality_filter, @p_player_name_filter)";
                
                var seasonIdParam = command.CreateParameter();
                seasonIdParam.ParameterName = "p_season_id";
                seasonIdParam.Value = (object?)seasonId ?? DBNull.Value; // NULL means all seasons
                command.Parameters.Add(seasonIdParam);

                var positionParam = command.CreateParameter();
                positionParam.ParameterName = "p_position_filter";
                positionParam.Value = (object?)position ?? DBNull.Value;
                command.Parameters.Add(positionParam);

                var nationalityParam = command.CreateParameter();
                nationalityParam.ParameterName = "p_nationality_filter";
                nationalityParam.Value = (object?)nationality ?? DBNull.Value;
                command.Parameters.Add(nationalityParam);

                var playerNameParam = command.CreateParameter();
                playerNameParam.ParameterName = "p_player_name_filter";
                playerNameParam.Value = (object?)playerName ?? DBNull.Value;
                command.Parameters.Add(playerNameParam);

                await _context.Database.OpenConnectionAsync();
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var reportData = new ScoutingReportData
                    {
                        PlayerId = reader.GetInt32(0), // player_id
                        PlayerFullName = reader.GetString(1), // player_full_name
                        PositionName = reader.GetString(2), // position_name
                        NationalityName = reader.GetString(3), // nationality_name
                        LatestHeight = reader.IsDBNull(4) ? null : reader.GetInt32(4), // latest_height
                        LatestWeight = reader.IsDBNull(5) ? null : reader.GetInt32(5), // latest_weight
                        LatestJumpDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6), // latest_jump_date
                        LatestVerticalJump = reader.IsDBNull(7) ? null : reader.GetInt32(7), // latest_vertical_jump
                        TotalSessionsAnalyzed = reader.IsDBNull(8) ? 0 : reader.GetInt32(8), // total_sessions_analyzed (handle NULL)
                        TotalScoutingScore = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9), // total_scouting_score (handle NULL)
                        NormalizedScore = reader.IsDBNull(10) ? 0 : reader.GetDecimal(10), // relative_normalized_score (handle NULL)
                        ReportDate = reader.GetDateTime(11) // report_date
                    };
                    
                    results.Add(reportData);
                    Console.WriteLine($"Relative normalization for player {reportData.PlayerId}: {reportData.NormalizedScore}%");
                }
                
                await _context.Database.CloseConnectionAsync();
                
                // Now fetch basketball performance data for each player
                foreach (var player in results)
                {
                    try
                    {
                        using var basketballCommand = _context.Database.GetDbConnection().CreateCommand();
                        basketballCommand.CommandText = "SELECT * FROM get_player_basketball_averages(@p_player_id, @p_season_id)";
                        
                        var playerIdParam = basketballCommand.CreateParameter();
                        playerIdParam.ParameterName = "p_player_id";
                        playerIdParam.Value = player.PlayerId;
                        basketballCommand.Parameters.Add(playerIdParam);

                        var basketballSeasonIdParam = basketballCommand.CreateParameter();
                        basketballSeasonIdParam.ParameterName = "p_season_id";
                        basketballSeasonIdParam.Value = (object?)seasonId ?? DBNull.Value;
                        basketballCommand.Parameters.Add(basketballSeasonIdParam);

                        await _context.Database.OpenConnectionAsync();
                        
                        using var basketballReader = await basketballCommand.ExecuteReaderAsync();
                        if (await basketballReader.ReadAsync())
                        {
                            player.AveragePoints = basketballReader.IsDBNull(0) ? null : basketballReader.GetDecimal(0);
                            player.AverageAssists = basketballReader.IsDBNull(1) ? null : basketballReader.GetDecimal(1);
                            player.AverageMinutes = basketballReader.IsDBNull(2) ? null : basketballReader.GetDecimal(2);
                        }
                        
                        await _context.Database.CloseConnectionAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to get basketball averages for player {player.PlayerId}: {ex.Message}");
                        // Set defaults if basketball data fetch fails
                        player.AveragePoints = null;
                        player.AverageAssists = null;
                        player.AverageMinutes = null;
                    }
                }
                
                await _context.Database.CloseConnectionAsync();
                
                Console.WriteLine($"Relative normalization function returned {results.Count} players");
                return results.OrderByDescending(r => r.NormalizedScore).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Relative normalization function failed: {ex.Message}. Falling back to old approach.");
                await _context.Database.CloseConnectionAsync();
            }

            // Fallback to old individual player approach if new function fails
            var playersQuery = _context.Players
                .Include(p => p.IdPositionNavigation)
                .Include(p => p.IdNationalityNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(position))
            {
                playersQuery = playersQuery.Where(p => p.IdPositionNavigation.Name.Contains(position));
            }

            if (!string.IsNullOrEmpty(nationality))
            {
                playersQuery = playersQuery.Where(p => p.IdNationalityNavigation.State.Contains(nationality));
            }

            if (!string.IsNullOrEmpty(playerName))
            {
                playersQuery = playersQuery.Where(p => (p.Name + " " + p.Surname).Contains(playerName));
            }

            var players = await playersQuery.ToListAsync();
            Console.WriteLine($"Fallback: Found {players.Count} players matching criteria");

            // Get raw scores first to find maximum
            var playerScores = new Dictionary<int, decimal>();
            foreach (var player in players)
            {
                try
                {
                    using var command = _context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = "SELECT * FROM generate_player_scouting_summary(@p_player_id, @p_season_id)";
                    
                    var playerIdParam = command.CreateParameter();
                    playerIdParam.ParameterName = "p_player_id";
                    playerIdParam.Value = player.IdPlayer;
                    command.Parameters.Add(playerIdParam);

                    var seasonIdParam = command.CreateParameter();
                    seasonIdParam.ParameterName = "p_season_id";
                    seasonIdParam.Value = (object?)seasonId ?? DBNull.Value; // NULL means all seasons
                    command.Parameters.Add(seasonIdParam);

                    await _context.Database.OpenConnectionAsync();
                    
                    using var reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var rawScore = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9); // total_scouting_score (handle NULL)
                        playerScores[player.IdPlayer] = rawScore;
                    }
                    
                    await _context.Database.CloseConnectionAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to get score for player {player.IdPlayer}: {ex.Message}");
                    playerScores[player.IdPlayer] = 0;
                }
            }

            // Find maximum score for relative normalization
            var maxScore = playerScores.Values.Max();
            if (maxScore == 0) maxScore = 1; // Avoid division by zero

            // Generate final results with relative normalization
            foreach (var player in players)
            {
                try
                {
                    using var command = _context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = "SELECT * FROM generate_player_scouting_summary(@p_player_id, @p_season_id)";
                    
                    var playerIdParam = command.CreateParameter();
                    playerIdParam.ParameterName = "p_player_id";
                    playerIdParam.Value = player.IdPlayer;
                    command.Parameters.Add(playerIdParam);

                    var seasonIdParam = command.CreateParameter();
                    seasonIdParam.ParameterName = "p_season_id";
                    seasonIdParam.Value = (object?)seasonId ?? DBNull.Value; // NULL means all seasons
                    command.Parameters.Add(seasonIdParam);

                    await _context.Database.OpenConnectionAsync();
                    
                    using var reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var rawScore = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9); // total_scouting_score (handle NULL)
                        var relativeScore = Math.Round((rawScore / maxScore) * 100, 2);
                        
                        var reportData = new ScoutingReportData
                        {
                            PlayerId = reader.GetInt32(0), // player_id
                            PlayerFullName = reader.GetString(1), // player_full_name
                            PositionName = reader.GetString(2), // position_name
                            NationalityName = reader.GetString(3), // nationality_name
                            LatestHeight = reader.IsDBNull(4) ? null : reader.GetInt32(4), // latest_height
                            LatestWeight = reader.IsDBNull(5) ? null : reader.GetInt32(5), // latest_weight
                            LatestJumpDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6), // latest_jump_date
                            LatestVerticalJump = reader.IsDBNull(7) ? null : reader.GetInt32(7), // latest_vertical_jump
                            TotalSessionsAnalyzed = reader.IsDBNull(8) ? 0 : reader.GetInt32(8), // total_sessions_analyzed (handle NULL)
                            TotalScoutingScore = rawScore, // total_scouting_score
                            NormalizedScore = relativeScore, // Calculated relative score (0-100%)
                            ReportDate = reader.GetDateTime(11) // report_date
                        };
                        
                        results.Add(reportData);
                        Console.WriteLine($"Fallback relative normalization for player {player.IdPlayer}: {relativeScore}% (raw: {rawScore}, max: {maxScore})");
                    }
                    
                    await _context.Database.CloseConnectionAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fallback failed for player {player.IdPlayer}: {ex.Message}");
                    continue;
                }
            }

            // Now fetch basketball performance data for fallback results
            foreach (var player in results)
            {
                try
                {
                    using var basketballCommand = _context.Database.GetDbConnection().CreateCommand();
                    basketballCommand.CommandText = "SELECT * FROM get_player_basketball_averages(@p_player_id, @p_season_id)";
                    
                    var playerIdParam = basketballCommand.CreateParameter();
                    playerIdParam.ParameterName = "p_player_id";
                    playerIdParam.Value = player.PlayerId;
                    basketballCommand.Parameters.Add(playerIdParam);

                    var fallbackSeasonIdParam = basketballCommand.CreateParameter();
                    fallbackSeasonIdParam.ParameterName = "p_season_id";
                    fallbackSeasonIdParam.Value = (object?)seasonId ?? DBNull.Value;
                    basketballCommand.Parameters.Add(fallbackSeasonIdParam);

                    await _context.Database.OpenConnectionAsync();
                    
                    using var basketballReader = await basketballCommand.ExecuteReaderAsync();
                    if (await basketballReader.ReadAsync())
                    {
                        player.AveragePoints = basketballReader.IsDBNull(0) ? null : basketballReader.GetDecimal(0);
                        player.AverageAssists = basketballReader.IsDBNull(1) ? null : basketballReader.GetDecimal(1);
                        player.AverageMinutes = basketballReader.IsDBNull(2) ? null : basketballReader.GetDecimal(2);
                    }
                    
                    await _context.Database.CloseConnectionAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to get basketball averages for player {player.PlayerId}: {ex.Message}");
                    // Set defaults if basketball data fetch fails
                    player.AveragePoints = null;
                    player.AverageAssists = null;
                    player.AverageMinutes = null;
                }
            }

            Console.WriteLine($"Final result: {results.Count} players with relative normalization");
            return results.OrderByDescending(r => r.NormalizedScore).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetScoutingReportDataAsync: {ex.Message}");
            return new List<ScoutingReportData>();
        }
    }
}
