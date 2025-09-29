using MediatR;
using Microsoft.Extensions.Configuration;
using Npgsql;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.Match.GetMatchPlayerStatistics;

public class GetMatchPlayerStatisticsHandler : IRequestHandler<GetMatchPlayerStatisticsQuery, Result<GetMatchPlayerStatisticsResponse>>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetMatchPlayerStatisticsHandler> _logger;

    public GetMatchPlayerStatisticsHandler(IConfiguration configuration, ILogger<GetMatchPlayerStatisticsHandler> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<GetMatchPlayerStatisticsResponse>> Handle(GetMatchPlayerStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting match player statistics for match ID: {MatchId}", request.MatchId);

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var playerStatistics = new List<PlayerEfficiencyDto>();

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var sql = "SELECT * FROM get_match_player_statistics(@p_match_id)";
            
            _logger.LogDebug("Executing SQL: {Sql} with MatchId: {MatchId}", sql, request.MatchId);

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@p_match_id", request.MatchId);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var playerDto = new PlayerEfficiencyDto
                {
                    PlayerId = Convert.ToInt32(reader["player_id"]),
                    TeamId = Convert.ToInt32(reader["team_id"]),
                    MatchId = Convert.ToInt32(reader["match_id"]),
                    PlayerName = reader["player_name"].ToString() ?? string.Empty,
                    MatchName = reader["match_name"].ToString() ?? string.Empty,
                    TotalPoints = Convert.ToInt32(reader["total_points"]),
                    TotalAssists = Convert.ToInt32(reader["total_assists"]),
                    TotalRebounds = Convert.ToInt32(reader["total_rebounds"]),
                    TotalSteals = Convert.ToInt32(reader["total_steals"]),
                    TotalBlocks = Convert.ToInt32(reader["total_blocks"]),
                    TotalFouls = Convert.ToInt32(reader["total_fouls"]),
                    Shooting2PMade = Convert.ToInt32(reader["shooting_2p_made"]),
                    Shooting2PAttempted = Convert.ToInt32(reader["shooting_2p_attempted"]),
                    Shooting2PPercentage = Convert.ToDecimal(reader["shooting_2p_percentage"]),
                    Shooting3PMade = Convert.ToInt32(reader["shooting_3p_made"]),
                    Shooting3PAttempted = Convert.ToInt32(reader["shooting_3p_attempted"]),
                    Shooting3PPercentage = Convert.ToDecimal(reader["shooting_3p_percentage"]),
                    FreeThrowsMade = Convert.ToInt32(reader["free_throws_made"]),
                    FreeThrowsAttempted = Convert.ToInt32(reader["free_throws_attempted"]),
                    FreeThrowPercentage = Convert.ToDecimal(reader["free_throw_percentage"]),
                    OffensiveRebounds = Convert.ToInt32(reader["offensive_rebounds"]),
                    DefensiveRebounds = Convert.ToInt32(reader["defensive_rebounds"]),
                    SubstitutionsCount = Convert.ToInt32(reader["substitutions_count"]),
                    EfficiencyRating = Convert.ToDecimal(reader["efficiency_rating"]),
                    PerformanceGrade = reader["performance_grade"].ToString() ?? string.Empty,
                    MinutesPlayed = Convert.ToInt32(reader["minutes_played"]),
                    PlusMinusRating = Convert.ToDecimal(reader["plus_minus_rating"])
                };

                playerStatistics.Add(playerDto);
                _logger.LogDebug("Added player statistics: {PlayerName} with efficiency: {Efficiency}", 
                    playerDto.PlayerName, playerDto.EfficiencyRating);
            }

            _logger.LogInformation("Retrieved {Count} player statistics for match {MatchId}", 
                playerStatistics.Count, request.MatchId);

            var response = new GetMatchPlayerStatisticsResponse
            {
                PlayerStatistics = playerStatistics,
                MatchId = request.MatchId,
                Message = $"Successfully retrieved statistics for {playerStatistics.Count} players"
            };

            return Result<GetMatchPlayerStatisticsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting match player statistics for match ID: {MatchId}", request.MatchId);
            return Result<GetMatchPlayerStatisticsResponse>.Failure($"Error retrieving player statistics: {ex.Message}");
        }
    }
}