using MediatR;
using Microsoft.Extensions.Configuration;
using Npgsql;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using Microsoft.Extensions.Logging;
using System.Data;

namespace match_service.src.Matches.Core.Application.Features.Match.GetCompleteMatchReport;

public class GetCompleteMatchReportHandler : IRequestHandler<GetCompleteMatchReportQuery, Result<GetCompleteMatchReportResponse>>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetCompleteMatchReportHandler> _logger;

    public GetCompleteMatchReportHandler(IConfiguration configuration, ILogger<GetCompleteMatchReportHandler> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<GetCompleteMatchReportResponse>> Handle(GetCompleteMatchReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Generating complete match report for match ID: {MatchId}", request.MatchId);

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            // Pozivamo glavnu PL/SQL funkciju za kompletan izveštaj sa explicit casting
            var sql = @"SELECT 
                        (general_info)::text as general_info,
                        (our_team_stats)::text as our_team_stats,
                        (opponent_team_stats)::text as opponent_team_stats,
                        (our_players_stats)::text as our_players_stats,
                        (opponent_players_stats)::text as opponent_players_stats
                      FROM generate_complete_match_report(@p_match_id)";
            
            _logger.LogDebug("Executing SQL: {Sql} with MatchId: {MatchId}", sql, request.MatchId);

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@p_match_id", request.MatchId);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                _logger.LogWarning("No match report data found for match ID: {MatchId}", request.MatchId);
                return Result<GetCompleteMatchReportResponse>.Failure($"No data found for match {request.MatchId}");
            }

            // Parsiranje složenog tipa iz PostgreSQL-a
            var response = new GetCompleteMatchReportResponse();

            // Parsiranje general_info composite type
            var generalInfoData = reader["general_info"] as string;
            if (!string.IsNullOrEmpty(generalInfoData))
            {
                response.GeneralInfo = ParseMatchGeneralInfo(generalInfoData);
            }

            // Parsiranje our_team_stats composite type
            var ourTeamStatsData = reader["our_team_stats"] as string;
            if (!string.IsNullOrEmpty(ourTeamStatsData))
            {
                response.OurTeamStats = ParseTeamMatchStats(ourTeamStatsData);
            }

            // Parsiranje opponent_team_stats composite type
            var opponentTeamStatsData = reader["opponent_team_stats"] as string;
            if (!string.IsNullOrEmpty(opponentTeamStatsData))
            {
                response.OpponentTeamStats = ParseTeamMatchStats(opponentTeamStatsData);
            }

            // Parsiranje our_players_stats array
            var ourPlayersData = reader["our_players_stats"] as string;
            if (!string.IsNullOrEmpty(ourPlayersData))
            {
                response.OurPlayersStats = ParsePlayersArray(ourPlayersData);
            }

            // Parsiranje opponent_players_stats array
            var opponentPlayersData = reader["opponent_players_stats"] as string;
            if (!string.IsNullOrEmpty(opponentPlayersData))
            {
                response.OpponentPlayersStats = ParsePlayersArray(opponentPlayersData);
            }

            // Osnovno čitanje
            response.TotalPlayersCount = response.OurPlayersStats.Count + response.OpponentPlayersStats.Count;
            response.MatchSummary = "Match statistics generated from database";
            response.Message = "Complete match report generated successfully";

            _logger.LogInformation("Successfully generated complete match report for match {MatchId}", request.MatchId);

            return Result<GetCompleteMatchReportResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating complete match report for match ID: {MatchId}", request.MatchId);
            return Result<GetCompleteMatchReportResponse>.Failure($"Error generating complete match report: {ex.Message}");
        }
    }

    private async Task<List<PlayerEfficiencyDto>> GetPlayersStatsAsync(NpgsqlConnection connection, int matchId, int teamId, CancellationToken cancellationToken)
    {
        var players = new List<PlayerEfficiencyDto>();

        try
        {
            using var command = new NpgsqlCommand("SELECT * FROM get_match_players_for_csharp(@p_match_id) WHERE team_id = @team_id", connection);
            command.Parameters.AddWithValue("@p_match_id", matchId);
            command.Parameters.AddWithValue("@team_id", teamId);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            
            while (await reader.ReadAsync(cancellationToken))
            {
                players.Add(new PlayerEfficiencyDto
                {
                    PlayerId = reader.GetInt32("player_id"),
                    TeamId = reader.GetInt32("team_id"),
                    MatchId = reader.GetInt32("match_id"),
                    PlayerName = reader.GetString("player_name"),
                    MatchName = reader.GetString("match_name"),
                    TotalPoints = reader.GetInt32("total_points"),
                    TotalAssists = reader.GetInt32("total_assists"),
                    TotalRebounds = reader.GetInt32("total_rebounds"),
                    TotalSteals = reader.GetInt32("total_steals"),
                    TotalBlocks = reader.GetInt32("total_blocks"),
                    TotalFouls = reader.GetInt32("total_fouls"),
                    Shooting2PMade = reader.GetInt32("shooting_2p_made"),
                    Shooting2PAttempted = reader.GetInt32("shooting_2p_attempted"),
                    Shooting2PPercentage = reader.GetDecimal("shooting_2p_percentage"),
                    Shooting3PMade = reader.GetInt32("shooting_3p_made"),
                    Shooting3PAttempted = reader.GetInt32("shooting_3p_attempted"),
                    Shooting3PPercentage = reader.GetDecimal("shooting_3p_percentage"),
                    FreeThrowsMade = reader.GetInt32("free_throws_made"),
                    FreeThrowsAttempted = reader.GetInt32("free_throws_attempted"),
                    FreeThrowPercentage = reader.GetDecimal("free_throw_percentage"),
                    OffensiveRebounds = reader.GetInt32("offensive_rebounds"),
                    DefensiveRebounds = reader.GetInt32("defensive_rebounds"),
                    SubstitutionsCount = reader.GetInt32("substitutions_count"),
                    EfficiencyRating = reader.GetDecimal("efficiency_rating"),
                    PerformanceGrade = reader.GetString("performance_grade"),
                    MinutesPlayed = reader.GetInt32("minutes_played"),
                    PlusMinusRating = reader.GetDecimal("plus_minus_rating")
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players stats for match {MatchId}, team {TeamId}", matchId, teamId);
        }

        return players;
    }

    // Stare metode koje više ne koristimo možemo ostaviti za kompatibilnost
    private MatchGeneralInfoDto ParseMatchGeneralInfo(string compositeData)
    {
        try
        {
            // PostgreSQL composite type format: (value1,value2,value3,...)
            // Potrebno je parsirati ovu strukturu
            var trimmed = compositeData.Trim('(', ')');
            var parts = SplitCompositeType(trimmed);

            return new MatchGeneralInfoDto
            {
                MatchId = SafeParseInt(parts, 0),
                MatchName = SafeParseString(parts, 1),
                ScheduledAt = SafeParseDateTime(parts, 2),
                Hall = SafeParseString(parts, 3),
                City = SafeParseString(parts, 4),
                State = SafeParseString(parts, 5),
                DurationMinutes = SafeParseInt(parts, 6),
                TotalEventsCount = SafeParseInt(parts, 7),
                HighestIndividualScore = SafeParseInt(parts, 8),
                LowestIndividualScore = SafeParseInt(parts, 9),
                TotalSubstitutions = SafeParseInt(parts, 10),
                TotalFouls = SafeParseInt(parts, 11),
                OurTeamId = SafeParseInt(parts, 12),
                OpponentTeamId = SafeParseInt(parts, 13),
                FinalScoreOur = SafeParseInt(parts, 14),
                FinalScoreOpponent = SafeParseInt(parts, 15)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing match general info composite type: {Data}", compositeData);
            return new MatchGeneralInfoDto();
        }
    }

    private TeamMatchStatsDto ParseTeamMatchStats(string compositeData)
    {
        try
        {
            var trimmed = compositeData.Trim('(', ')');
            var parts = SplitCompositeType(trimmed);

            return new TeamMatchStatsDto
            {
                TeamId = SafeParseInt(parts, 0),
                TeamName = SafeParseString(parts, 1),
                TotalPoints = SafeParseInt(parts, 2),
                TotalFieldGoalsMade = SafeParseInt(parts, 3),
                TotalFieldGoalsAttempted = SafeParseInt(parts, 4),
                FieldGoalPercentage = SafeParseDecimal(parts, 5),
                Total2PMade = SafeParseInt(parts, 6),
                Total2PAttempted = SafeParseInt(parts, 7),
                TwoPointPercentage = SafeParseDecimal(parts, 8),
                Total3PMade = SafeParseInt(parts, 9),
                Total3PAttempted = SafeParseInt(parts, 10),
                ThreePointPercentage = SafeParseDecimal(parts, 11),
                TotalFreeThrowsMade = SafeParseInt(parts, 12),
                TotalFreeThrowsAttempted = SafeParseInt(parts, 13),
                FreeThrowPercentage = SafeParseDecimal(parts, 14),
                TotalRebounds = SafeParseInt(parts, 15),
                TotalOffensiveRebounds = SafeParseInt(parts, 16),
                TotalDefensiveRebounds = SafeParseInt(parts, 17),
                TotalAssists = SafeParseInt(parts, 18),
                TotalSteals = SafeParseInt(parts, 19),
                TotalBlocks = SafeParseInt(parts, 20),
                TotalFouls = SafeParseInt(parts, 21),
                TeamEfficiencyRating = SafeParseDecimal(parts, 22),
                ActivePlayersCount = SafeParseInt(parts, 23),
                SubstitutionsCount = SafeParseInt(parts, 24),
                AvgPlayerEfficiency = SafeParseDecimal(parts, 25),
                BestPlayerName = SafeParseString(parts, 26),
                BestPlayerEfficiency = SafeParseDecimal(parts, 27)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing team match stats composite type: {Data}", compositeData);
            return new TeamMatchStatsDto();
        }
    }

    private string[] SplitCompositeType(string data)
    {
        // Jednostavno splitovanje po zapetama, ali moraće se proširiti za quoted stringove
        var parts = new List<string>();
        var current = "";
        var inQuotes = false;
        var escapeNext = false;

        for (int i = 0; i < data.Length; i++)
        {
            var c = data[i];
            
            if (escapeNext)
            {
                current += c;
                escapeNext = false;
                continue;
            }

            if (c == '\\')
            {
                escapeNext = true;
                continue;
            }

            if (c == '"' && !escapeNext)
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                parts.Add(current.Trim());
                current = "";
                continue;
            }

            current += c;
        }

        if (!string.IsNullOrEmpty(current))
        {
            parts.Add(current.Trim());
        }

        return parts.ToArray();
    }

    private int SafeParseInt(string[] parts, int index, int defaultValue = 0)
    {
        if (index >= parts.Length || string.IsNullOrEmpty(parts[index]) || parts[index] == "null")
            return defaultValue;
        return int.TryParse(parts[index], out var result) ? result : defaultValue;
    }

    private int? SafeParseNullableInt(string[] parts, int index)
    {
        if (index >= parts.Length || string.IsNullOrEmpty(parts[index]) || parts[index] == "null")
            return null;
        return int.TryParse(parts[index], out var result) ? result : null;
    }

    private decimal SafeParseDecimal(string[] parts, int index, decimal defaultValue = 0m)
    {
        if (index >= parts.Length || string.IsNullOrEmpty(parts[index]) || parts[index] == "null")
            return defaultValue;
        return decimal.TryParse(parts[index], out var result) ? result : defaultValue;
    }

    private string SafeParseString(string[] parts, int index, string defaultValue = "")
    {
        if (index >= parts.Length || parts[index] == "null")
            return defaultValue;
        return parts[index]?.Trim('"') ?? defaultValue;
    }

    private DateTime SafeParseDateTime(string[] parts, int index)
    {
        if (index >= parts.Length || string.IsNullOrEmpty(parts[index]) || parts[index] == "null")
            return DateTime.MinValue;
        return DateTime.TryParse(parts[index]?.Trim('"'), out var result) ? result : DateTime.MinValue;
    }

    private List<PlayerEfficiencyDto> ParsePlayersArray(string arrayData)
    {
        var players = new List<PlayerEfficiencyDto>();
        
        try
        {
            // PostgreSQL array format: {"(values...)","(values...)"}
            if (string.IsNullOrEmpty(arrayData) || arrayData == "{}" || arrayData == "null")
                return players;

            var trimmed = arrayData.Trim('{', '}');
            if (string.IsNullOrEmpty(trimmed))
                return players;

            // Split by "," but respect quoted strings
            var playerStrings = SplitArrayElements(trimmed);

            foreach (var playerString in playerStrings)
            {
                var cleanString = playerString.Trim('"');
                if (!string.IsNullOrEmpty(cleanString))
                {
                    var player = ParsePlayerFromCompositeString(cleanString);
                    if (player != null)
                        players.Add(player);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing players array: {Data}", arrayData);
        }

        return players;
    }

    private string[] SplitArrayElements(string data)
    {
        var elements = new List<string>();
        var current = "";
        var inQuotes = false;
        var parenLevel = 0;

        for (int i = 0; i < data.Length; i++)
        {
            var c = data[i];

            if (c == '"' && (i == 0 || data[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
                current += c;
            }
            else if (c == '(' && !inQuotes)
            {
                parenLevel++;
                current += c;
            }
            else if (c == ')' && !inQuotes)
            {
                parenLevel--;
                current += c;
            }
            else if (c == ',' && !inQuotes && parenLevel == 0)
            {
                elements.Add(current.Trim());
                current = "";
            }
            else
            {
                current += c;
            }
        }

        if (!string.IsNullOrEmpty(current))
            elements.Add(current.Trim());

        return elements.ToArray();
    }

    private PlayerEfficiencyDto? ParsePlayerFromCompositeString(string compositeData)
    {
        try
        {
            var trimmed = compositeData.Trim('(', ')');
            var parts = SplitCompositeType(trimmed);

            return new PlayerEfficiencyDto
            {
                PlayerId = SafeParseInt(parts, 0),
                TeamId = SafeParseInt(parts, 1),
                MatchId = SafeParseInt(parts, 2),
                PlayerName = SafeParseString(parts, 3),
                MatchName = SafeParseString(parts, 4),
                TotalPoints = SafeParseInt(parts, 5),
                TotalAssists = SafeParseInt(parts, 6),
                TotalRebounds = SafeParseInt(parts, 7),
                TotalSteals = SafeParseInt(parts, 8),
                TotalBlocks = SafeParseInt(parts, 9),
                TotalFouls = SafeParseInt(parts, 10),
                Shooting2PMade = SafeParseInt(parts, 11),
                Shooting2PAttempted = SafeParseInt(parts, 12),
                Shooting2PPercentage = SafeParseDecimal(parts, 13),
                Shooting3PMade = SafeParseInt(parts, 14),
                Shooting3PAttempted = SafeParseInt(parts, 15),
                Shooting3PPercentage = SafeParseDecimal(parts, 16),
                FreeThrowsMade = SafeParseInt(parts, 17),
                FreeThrowsAttempted = SafeParseInt(parts, 18),
                FreeThrowPercentage = SafeParseDecimal(parts, 19),
                OffensiveRebounds = SafeParseInt(parts, 20),
                DefensiveRebounds = SafeParseInt(parts, 21),
                SubstitutionsCount = SafeParseInt(parts, 22),
                EfficiencyRating = SafeParseDecimal(parts, 23),
                PerformanceGrade = SafeParseString(parts, 24),
                MinutesPlayed = SafeParseInt(parts, 25),
                PlusMinusRating = SafeParseDecimal(parts, 26)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing player composite type: {Data}", compositeData);
            return null;
        }
    }
}