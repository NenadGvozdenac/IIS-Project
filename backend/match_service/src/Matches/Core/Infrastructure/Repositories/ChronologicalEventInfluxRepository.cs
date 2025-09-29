using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace match_service.src.Matches.Core.Infrastructure.Repositories
{
    public class ChronologicalEventInfluxRepository : IChronologicalEventInfluxRepository
    {
        private readonly InfluxDBClient _influxDBClient;
        private readonly ILogger<ChronologicalEventInfluxRepository> _logger;
        private readonly string _bucket;
        private readonly string _org;

        public ChronologicalEventInfluxRepository(
            InfluxDBClient influxDBClient, 
            IConfiguration configuration,
            ILogger<ChronologicalEventInfluxRepository> logger)
        {
            _influxDBClient = influxDBClient;
            _logger = logger;
            _bucket = configuration["InfluxDB:Bucket"] ?? "events_bucket";
            _org = configuration["InfluxDB:Organization"] ?? "basketball_org";
        }

        public async Task<bool> WriteEventAsync(ChronologicalEventInflux chronologicalEvent)
        {
            try
            {
                var writeApi = _influxDBClient.GetWriteApiAsync();
                var point = PointData
                    .Measurement("basketball_events")
                    .Tag("match_id", chronologicalEvent.MatchId)
                    .Tag("event_category", chronologicalEvent.EventCategory)
                    .Tag("event_type", chronologicalEvent.EventType)
                    .Tag("period", chronologicalEvent.Period)
                    .Tag("team_id", chronologicalEvent.TeamId ?? "")
                    .Tag("player_id", chronologicalEvent.PlayerId ?? "")
                    .Field("event_id", chronologicalEvent.EventId)
                    .Field("player_name", chronologicalEvent.PlayerName ?? "")
                    .Field("period_time", chronologicalEvent.PeriodTime ?? 0)
                    .Field("notes", chronologicalEvent.Notes ?? "")
                    .Field("our_points", chronologicalEvent.OurPoints ?? 0)
                    .Field("opponent_points", chronologicalEvent.OpponentPoints ?? 0)
                    .Field("point_difference", chronologicalEvent.PointDifference ?? 0)
                    .Timestamp(chronologicalEvent.Timestamp, WritePrecision.Ms);

                await writeApi.WritePointAsync(point, _bucket, _org);
                _logger.LogInformation("Successfully wrote event {EventId} for match {MatchId}", chronologicalEvent.EventId, chronologicalEvent.MatchId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write event {EventId} for match {MatchId}", chronologicalEvent.EventId, chronologicalEvent.MatchId);
                return false;
            }
        }

        public async Task<bool> WriteEventsAsync(IEnumerable<ChronologicalEventInflux> chronologicalEvents)
        {
            try
            {
                var writeApi = _influxDBClient.GetWriteApiAsync();
                var points = chronologicalEvents.Select(e => PointData
                    .Measurement("basketball_events")
                    .Tag("match_id", e.MatchId)
                    .Tag("event_category", e.EventCategory)
                    .Tag("event_type", e.EventType)
                    .Tag("period", e.Period)
                    .Tag("team_id", e.TeamId ?? "")
                    .Tag("player_id", e.PlayerId ?? "")
                    .Field("event_id", e.EventId)
                    .Field("player_name", e.PlayerName ?? "")
                    .Field("period_time", e.PeriodTime ?? 0)
                    .Field("notes", e.Notes ?? "")
                    .Field("our_points", e.OurPoints ?? 0)
                    .Field("opponent_points", e.OpponentPoints ?? 0)
                    .Field("point_difference", e.PointDifference ?? 0)
                    .Timestamp(e.Timestamp, WritePrecision.Ms));

                await writeApi.WritePointsAsync(points.ToList(), _bucket, _org);
                _logger.LogInformation("Successfully wrote {Count} events", chronologicalEvents.Count());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write batch of {Count} events", chronologicalEvents.Count());
                return false;
            }
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetEventsByMatchIdAsync(string matchId)
        {
            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1y)
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetEventsByMatchIdAndCategoryAsync(string matchId, string eventCategory)
        {
            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1y)
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> filter(fn: (r) => r.event_category == ""{eventCategory}"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetEventsByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            var startTimeRfc = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            var endTimeRfc = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetEventsByMatchAndTimeRangeAsync(string matchId, DateTime startTime, DateTime endTime)
        {
            var startTimeRfc = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            var endTimeRfc = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetEventsByPlayerAsync(string playerId, DateTime? startTime = null, DateTime? endTime = null)
        {
            var rangeClause = BuildRangeClause(startTime, endTime);

            var flux = $@"
                from(bucket: ""{_bucket}"")
                  {rangeClause}
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.player_id == ""{playerId}"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetEventsByTeamAsync(string teamId, DateTime? startTime = null, DateTime? endTime = null)
        {
            var rangeClause = BuildRangeClause(startTime, endTime);

            var flux = $@"
                from(bucket: ""{_bucket}"")
                  {rangeClause}
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.team_id == ""{teamId}"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetEventsByTypeAsync(string eventType, DateTime? startTime = null, DateTime? endTime = null)
        {
            var rangeClause = BuildRangeClause(startTime, endTime);

            var flux = $@"
                from(bucket: ""{_bucket}"")
                  {rangeClause}
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.event_type == ""{eventType}"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetScoreEvolutionAsync(string matchId)
        {
            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1y)
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> filter(fn: (r) => r._field == ""our_points"" or r._field == ""opponent_points"" or r._field == ""point_difference"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<IEnumerable<ChronologicalEventInflux>> GetEventsByPeriodAsync(string matchId, string period)
        {
            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1y)
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> filter(fn: (r) => r.period == ""{period}"")
                  |> sort(columns: [""_time""])";

            return await ExecuteQueryAsync(flux);
        }

        public async Task<Dictionary<string, int>> GetEventCountsByTypeAsync(string matchId)
        {
            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1y)
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> filter(fn: (r) => r._field == ""event_id"")
                  |> group(columns: [""event_type""])
                  |> count()";

            try
            {
                var queryApi = _influxDBClient.GetQueryApi();
                var tables = await queryApi.QueryAsync(flux, _org);

                var eventCounts = new Dictionary<string, int>();
                foreach (var table in tables)
                {
                    foreach (var record in table.Records)
                    {
                        var eventType = record.GetValueByKey("event_type")?.ToString() ?? "unknown";
                        var count = Convert.ToInt32(record.GetValue());
                        eventCounts[eventType] = count;
                    }
                }

                return eventCounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get event counts for match {MatchId}", matchId);
                return new Dictionary<string, int>();
            }
        }

        public async Task<bool> DeleteEventsByMatchIdAsync(string matchId)
        {
            try
            {
                var deleteApi = _influxDBClient.GetDeleteApi();
                var start = DateTime.UtcNow.AddYears(-10);
                var stop = DateTime.UtcNow.AddDays(1);
                var predicate = $"match_id=\"{matchId}\"";

                await deleteApi.Delete(start, stop, predicate, _bucket, _org);
                _logger.LogInformation("Successfully deleted events for match {MatchId}", matchId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete events for match {MatchId}", matchId);
                return false;
            }
        }

        public async Task<bool> DeleteEventsOlderThanAsync(DateTime cutoffDate)
        {
            try
            {
                var deleteApi = _influxDBClient.GetDeleteApi();
                var start = DateTime.UtcNow.AddYears(-10);
                var stop = cutoffDate;
                var predicate = "_measurement=\"basketball_events\"";

                await deleteApi.Delete(start, stop, predicate, _bucket, _org);
                _logger.LogInformation("Successfully deleted events older than {CutoffDate}", cutoffDate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete events older than {CutoffDate}", cutoffDate);
                return false;
            }
        }

        // Saga deletion method
        public async Task<int> DeleteEventsByPlayerAndTeamAsync(int playerId, int teamId)
        {
            try
            {
                // First, count the events to be deleted
                var countFlux = $@"
                    from(bucket: ""{_bucket}"")
                      |> range(start: -10y)
                      |> filter(fn: (r) => r[""_measurement""] == ""basketball_events"")
                      |> filter(fn: (r) => r[""player_id""] == ""{playerId}"")
                      |> filter(fn: (r) => r[""team_id""] == ""{teamId}"")
                      |> count()";

                var queryApi = _influxDBClient.GetQueryApi();
                var countResult = await queryApi.QueryAsync(countFlux, _org);
                var eventsCount = 0;

                foreach (var table in countResult)
                {
                    foreach (var record in table.Records)
                    {
                        if (record.GetValue() != null && int.TryParse(record.GetValue().ToString(), out var count))
                        {
                            eventsCount += count;
                        }
                    }
                }

                // Delete the events
                var deleteApi = _influxDBClient.GetDeleteApi();
                var start = DateTime.UtcNow.AddYears(-10);
                var stop = DateTime.UtcNow.AddDays(1);
                var predicate = $"player_id=\"{playerId}\" and team_id=\"{teamId}\"";

                await deleteApi.Delete(start, stop, predicate, _bucket, _org);
                _logger.LogInformation("Successfully deleted {Count} events for player {PlayerId} and team {TeamId}", eventsCount, playerId, teamId);
                return eventsCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete events for player {PlayerId} and team {TeamId}", playerId, teamId);
                return 0;
            }
        }

        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                // Simple ping test using a basic query
                var flux = $@"
                    from(bucket: ""{_bucket}"")
                      |> range(start: -1m)
                      |> limit(n: 1)";

                var queryApi = _influxDBClient.GetQueryApi();
                await queryApi.QueryAsync(flux, _org);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InfluxDB connection check failed");
                return false;
            }
        }

        // SLOŽEN UPIT 1: Kombinuje filtriranje, grupisanje, agregaciju i sortiranje
        // Analiza efikasnosti igrača po periodima (prosečni poeni, asistencije, faul-ovi)
        public async Task<IEnumerable<dynamic>> GetAdvancedMatchStatisticsAsync(string matchId)
        {
            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1y)
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> filter(fn: (r) => r.event_category == ""personal"")
                  |> filter(fn: (r) => r.event_type == ""+2p"" or r.event_type == ""+3p"" or r.event_type == ""+ft"" or r.event_type == ""assist"" or r.event_type == ""foul"")
                  |> filter(fn: (r) => r._field == ""event_id"")
                  |> group(columns: [""period"", ""event_type"", ""team_id""])
                  |> count()
                  |> group()
                  |> sort(columns: [""period"", ""team_id"", ""event_type""])";

            try
            {
                var queryApi = _influxDBClient.GetQueryApi();
                var tables = await queryApi.QueryAsync(flux, _org);
                
                var results = new List<dynamic>();
                foreach (var table in tables)
                {
                    foreach (var record in table.Records)
                    {
                        results.Add(new
                        {
                            Period = record.GetValueByKey("period"),
                            EventType = record.GetValueByKey("event_type"),
                            TeamId = record.GetValueByKey("team_id"),
                            Count = Convert.ToInt32(record.GetValue()),
                            Timestamp = record.GetTime()
                        });
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get advanced match statistics for {MatchId}", matchId);
                return new List<dynamic>();
            }
        }

        // SLOŽEN UPIT 2: Poređenje performansi igrača sa agregatnim funkcijama
        // Rangiranje igrača po ukupnim poenima sa detaljima po tipovima šuta
        public async Task<IEnumerable<dynamic>> GetPlayerPerformanceComparisonAsync(string matchId)
        {
            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1y)
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> filter(fn: (r) => r.event_category == ""personal"")
                  |> filter(fn: (r) => r._field == ""event_id"")
                  |> group(columns: [""player_id"", ""event_type""])
                  |> count()
                  |> group()
                  |> sort(columns: [""player_id"", ""event_type""])";

            try
            {
                var queryApi = _influxDBClient.GetQueryApi();
                var tables = await queryApi.QueryAsync(flux, _org);
                
                var results = new List<dynamic>();
                foreach (var table in tables)
                {
                    foreach (var record in table.Records)
                    {
                        results.Add(new
                        {
                            PlayerId = record.GetValueByKey("player_id"),
                            EventType = record.GetValueByKey("event_type"),
                            TotalEvents = Convert.ToInt32(record.GetValue()),
                            Rank = results.Count + 1
                        });
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get player performance comparison for {MatchId}", matchId);
                return new List<dynamic>();
            }
        }

        // SLOŽEN UPIT 3: Season per-player averages across matches
        // Analiza performansi igrača kroz više utakmica sa agregatnim statistikama
        public async Task<IEnumerable<dynamic>> GetSeasonPlayerAveragesAsync(DateTime startTime, DateTime endTime, string? teamId = null, int minMatches = 1)
        {
            // Convert to UTC and use ISO format without 'Z' suffix
            var startTimeRfc = startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            var endTimeRfc = endTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            
            _logger.LogInformation("Season player averages query - Start: {StartTime}, End: {EndTime}, TeamId: {TeamId}, MinMatches: {MinMatches}", 
                startTimeRfc, endTimeRfc, teamId ?? "ALL", minMatches);
            
            var teamFilter = !string.IsNullOrEmpty(teamId) ? $@"|> filter(fn: (r) => r.team_id == ""{teamId}"")" : "";

            // First try a simple query to see if we have any data at all
            var testFlux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.event_category == ""personal"")
                  {teamFilter}
                  |> limit(n: 5)";
                  
            _logger.LogInformation("Test query to check for data: {TestFlux}", testFlux);
            
            try 
            {
                var testQueryApi = _influxDBClient.GetQueryApi();
                var testTables = await testQueryApi.QueryAsync(testFlux, _org);
                _logger.LogInformation("Test query returned {TableCount} tables with {TotalRecords} total records", 
                    testTables.Count, testTables.Sum(t => t.Records.Count));
            }
            catch (Exception testEx)
            {
                _logger.LogError(testEx, "Test query failed");
            }

            // Simplified query - single step approach to avoid scope issues  
            var flux = $@"
                import ""math""
                
                data = from(bucket: ""{_bucket}"")
                  |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.event_category == ""personal"")
                  |> filter(fn: (r) => r.event_type == ""+2p"" or r.event_type == ""+3p"" or r.event_type == ""+ft"" or r.event_type == ""assist"" or r.event_type == ""foul"")
                  |> filter(fn: (r) => r._field == ""event_id"")
                  {teamFilter}
                  |> group(columns: [""player_id"", ""match_id"", ""event_type""])
                  |> count()
                  |> group()

                // Step 2: Pivot to get event types as columns per match
                pivoted = data
                  |> pivot(rowKey: [""player_id"", ""match_id""], columnKey: [""event_type""], valueColumn: ""_value"")
                  |> map(fn: (r) => ({{
                    player_id: r.player_id,
                    match_id: r.match_id,
                    two_pointers: if exists r[""+2p""] then r[""+2p""] else 0,
                    three_pointers: if exists r[""+3p""] then r[""+3p""] else 0,
                    free_throws: if exists r[""+ft""] then r[""+ft""] else 0,
                    assists: if exists r.assist then r.assist else 0,
                    fouls: if exists r.foul then r.foul else 0
                  }}))
                  |> map(fn: (r) => ({{
                    r with
                    total_points: (r.two_pointers * 2) + (r.three_pointers * 3) + r.free_throws
                  }}))

                // Step 3: Aggregate per player across all matches
                pivoted
                  |> group(columns: [""player_id""])
                  |> reduce(
                    identity: {{
                      player_id: """",
                      matches_played: 0,
                      total_points: 0.0,
                      total_assists: 0.0,
                      total_fouls: 0.0,
                      points_sum_squares: 0.0
                    }},
                    fn: (r, accumulator) => ({{
                      player_id: r.player_id,
                      matches_played: accumulator.matches_played + 1,
                      total_points: accumulator.total_points + float(v: r.total_points),
                      total_assists: accumulator.total_assists + float(v: r.assists),
                      total_fouls: accumulator.total_fouls + float(v: r.fouls),
                      points_sum_squares: accumulator.points_sum_squares + (float(v: r.total_points) * float(v: r.total_points))
                    }})
                  )
                  |> filter(fn: (r) => r.matches_played >= {minMatches})
                  |> map(fn: (r) => ({{
                    r with
                    avg_points: r.total_points / float(v: r.matches_played),
                    avg_assists: r.total_assists / float(v: r.matches_played),
                    avg_fouls: r.total_fouls / float(v: r.matches_played)
                  }}))
                  |> map(fn: (r) => ({{
                    r with
                    points_variance: (r.points_sum_squares / float(v: r.matches_played)) - (r.avg_points * r.avg_points)
                  }}))
                  |> map(fn: (r) => ({{
                    r with
                    points_stddev: if r.points_variance >= 0.0 then math.sqrt(x: r.points_variance) else 0.0
                  }}))
                  |> sort(columns: [""avg_points""], desc: true)";

            try
            {
                _logger.LogInformation("Executing season player averages flux query: {Flux}", flux);
                
                var queryApi = _influxDBClient.GetQueryApi();
                var tables = await queryApi.QueryAsync(flux, _org);
                
                _logger.LogInformation("Query returned {TableCount} tables", tables.Count);
                
                var results = new List<dynamic>();
                foreach (var table in tables)
                {
                    _logger.LogInformation("Processing table with {RecordCount} records", table.Records.Count);
                    
                    foreach (var record in table.Records)
                    {
                        var playerId = record.GetValueByKey("player_id");
                        var matchesPlayed = record.GetValueByKey("matches_played");
                        var avgPoints = record.GetValueByKey("avg_points");
                        
                        _logger.LogDebug("Processing record - PlayerId: {PlayerId}, Matches: {Matches}, AvgPoints: {Points}", 
                            playerId, matchesPlayed, avgPoints);
                        
                        results.Add(new
                        {
                            PlayerId = playerId,
                            MatchesPlayed = Convert.ToInt32(matchesPlayed ?? 0),
                            AvgPoints = Math.Round(Convert.ToDouble(avgPoints ?? 0), 2),
                            AvgAssists = Math.Round(Convert.ToDouble(record.GetValueByKey("avg_assists") ?? 0), 2),
                            AvgFouls = Math.Round(Convert.ToDouble(record.GetValueByKey("avg_fouls") ?? 0), 2),
                            PointsStdDev = Math.Round(Convert.ToDouble(record.GetValueByKey("points_stddev") ?? 0), 2),
                            TotalPoints = Convert.ToDouble(record.GetValueByKey("total_points") ?? 0),
                            TotalAssists = Convert.ToDouble(record.GetValueByKey("total_assists") ?? 0),
                            TotalFouls = Convert.ToDouble(record.GetValueByKey("total_fouls") ?? 0)
                        });
                    }
                }
                
                _logger.LogInformation("Successfully processed {ResultCount} player averages", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get season player averages for period {StartTime} to {EndTime}", startTime, endTime);
                return new List<dynamic>();
            }
        }

        private async Task<IEnumerable<ChronologicalEventInflux>> ExecuteQueryAsync(string flux)
        {
            try
            {
                var queryApi = _influxDBClient.GetQueryApi();
                var tables = await queryApi.QueryAsync(flux, _org);

                var eventDict = new Dictionary<string, ChronologicalEventInflux>();
                
                foreach (var table in tables)
                {
                    foreach (var record in table.Records)
                    {
                        var timestamp = record.GetTime()?.ToDateTimeUtc() ?? DateTime.UtcNow;
                        var matchId = record.GetValueByKey("match_id")?.ToString() ?? "";
                        var eventCategory = record.GetValueByKey("event_category")?.ToString() ?? "";
                        var eventType = record.GetValueByKey("event_type")?.ToString() ?? "";
                        var period = record.GetValueByKey("period")?.ToString() ?? "";
                        var teamId = record.GetValueByKey("team_id")?.ToString();
                        var playerId = record.GetValueByKey("player_id")?.ToString();
                        
                        // Create unique key for grouping records of the same event
                        var eventKey = $"{timestamp:yyyy-MM-ddTHH:mm:ss.fff}_{matchId}_{eventCategory}_{eventType}_{period}_{teamId}_{playerId}";
                        
                        // Get or create event
                        if (!eventDict.ContainsKey(eventKey))
                        {
                            eventDict[eventKey] = new ChronologicalEventInflux
                            {
                                Timestamp = timestamp,
                                MatchId = matchId,
                                EventCategory = eventCategory,
                                EventType = eventType,
                                Period = period,
                                TeamId = teamId,
                                PlayerId = playerId
                            };
                        }

                        var chronologicalEvent = eventDict[eventKey];

                        // Handle different field types
                        var field = record.GetField();
                        var value = record.GetValue();

                        switch (field)
                        {
                            case "event_id":
                                chronologicalEvent.EventId = Convert.ToInt32(value);
                                break;
                            case "player_name":
                                chronologicalEvent.PlayerName = value?.ToString();
                                break;
                            case "period_time":
                                chronologicalEvent.PeriodTime = Convert.ToInt32(value);
                                break;
                            case "notes":
                                chronologicalEvent.Notes = value?.ToString();
                                break;
                            case "our_points":
                                chronologicalEvent.OurPoints = Convert.ToInt32(value);
                                break;
                            case "opponent_points":
                                chronologicalEvent.OpponentPoints = Convert.ToInt32(value);
                                break;
                            case "point_difference":
                                chronologicalEvent.PointDifference = Convert.ToInt32(value);
                                break;
                        }
                    }
                }

                return eventDict.Values.OrderBy(e => e.Timestamp).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute query: {Flux}", flux);
                return new List<ChronologicalEventInflux>();
            }
        }

        // INFLUX REPORT 1: Lista događaja za određenu utakmicu pri čemu je event_type '+2p', '+3p', '+ft' i event_category je 'personal' sortirano po vremenu
        public async Task<IEnumerable<dynamic>> GetMatchScoringEventsInfluxReportAsync(string matchId)
        {
            try
            {
                var flux = $@"
                    from(bucket: ""{_bucket}"")
                      |> range(start: -1y)
                      |> filter(fn: (r) => r._measurement == ""basketball_events"")
                      |> filter(fn: (r) => r.match_id == ""{matchId}"")
                      |> filter(fn: (r) => r.event_category == ""personal"")
                      |> filter(fn: (r) => r.event_type == ""+2p"" or r.event_type == ""+3p"" or r.event_type == ""+ft"")
                      |> filter(fn: (r) => r._field == ""event_id"" or r._field == ""player_name"" or r._field == ""period_time"")
                      |> pivot(rowKey: [""_time"", ""match_id"", ""player_id"", ""event_type"", ""period""], columnKey: [""_field""], valueColumn: ""_value"")
                      |> sort(columns: [""_time""])
                ";

                var queryApi = _influxDBClient.GetQueryApi();
                var tables = await queryApi.QueryAsync(flux, _org);
                var results = new List<dynamic>();

                foreach (var table in tables)
                {
                    foreach (var record in table.Records)
                    {
                        results.Add(new
                        {
                            Timestamp = record.GetTime()?.ToDateTimeUtc(),
                            MatchId = record.GetValueByKey("match_id")?.ToString(),
                            PlayerId = record.GetValueByKey("player_id")?.ToString(),
                            PlayerName = record.GetValueByKey("player_name")?.ToString(),
                            EventType = record.GetValueByKey("event_type")?.ToString(),
                            Period = record.GetValueByKey("period")?.ToString(),
                            PeriodTime = Convert.ToInt32(record.GetValueByKey("period_time") ?? 0),
                            EventId = Convert.ToInt32(record.GetValueByKey("event_id") ?? 0)
                        });
                    }
                }
                return results.OrderBy(r => r.Timestamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get match scoring events for {MatchId}", matchId);
                return new List<dynamic>();
            }
        }

        // INFLUX REPORT 2: Prikaz broja događaja za svaki event_type (gde je event_category personal), grupisano po match-u za određenog igrača
        public async Task<IEnumerable<dynamic>> GetPlayerEventCountsInfluxReportAsync(string playerId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var startTimeRfc = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                var endTimeRfc = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

                var flux = $@"
                    // Get player name first
                    playerNameData = from(bucket: ""{_bucket}"")
                      |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                      |> filter(fn: (r) => r._measurement == ""basketball_events"")
                      |> filter(fn: (r) => r.player_id == ""{playerId}"")
                      |> filter(fn: (r) => r._field == ""player_name"")
                      |> limit(n: 1)
                      |> keep(columns: [""_value""])
                      |> rename(columns: {{_value: ""player_name""}})

                    // Get event counts
                    eventCounts = from(bucket: ""{_bucket}"")
                      |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                      |> filter(fn: (r) => r._measurement == ""basketball_events"")
                      |> filter(fn: (r) => r.player_id == ""{playerId}"")
                      |> filter(fn: (r) => r.event_category == ""personal"")
                      |> filter(fn: (r) => r._field == ""event_id"")
                      |> group(columns: [""match_id"", ""event_type""])
                      |> count()
                      |> group()
                      |> pivot(rowKey: [""match_id""], columnKey: [""event_type""], valueColumn: ""_value"")
                      |> map(fn: (r) => ({{
                        match_id: r.match_id,
                        player_id: ""{playerId}"",
                        two_pointers: if exists r[""+2p""] then r[""+2p""] else 0,
                        three_pointers: if exists r[""+3p""] then r[""+3p""] else 0,
                        free_throws: if exists r[""+ft""] then r[""+ft""] else 0,
                        assists: if exists r.assist then r.assist else 0,
                        fouls: if exists r.foul then r.foul else 0,
                        steals: if exists r.steal then r.steal else 0,
                        blocks: if exists r.block then r.block else 0,
                        reb_off: if exists r[""reb of""] then r[""reb of""] else 0,
                        reb_def: if exists r[""reb def""] then r[""reb def""] else 0
                      }}))
                      |> sort(columns: [""match_id""])

                    eventCounts
                ";

                var queryApi = _influxDBClient.GetQueryApi();
                var tables = await queryApi.QueryAsync(flux, _org);
                var results = new List<dynamic>();

                // First get player name
                string playerName = "";
                var playerNameFlux = $@"
                    from(bucket: ""{_bucket}"")
                      |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                      |> filter(fn: (r) => r._measurement == ""basketball_events"")
                      |> filter(fn: (r) => r.player_id == ""{playerId}"")
                      |> filter(fn: (r) => r._field == ""player_name"")
                      |> limit(n: 1)
                ";

                try
                {
                    var playerNameTables = await queryApi.QueryAsync(playerNameFlux, _org);
                    foreach (var table in playerNameTables)
                    {
                        foreach (var record in table.Records)
                        {
                            playerName = record.GetValue()?.ToString() ?? "";
                            break;
                        }
                        if (!string.IsNullOrEmpty(playerName)) break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get player name for {PlayerId}", playerId);
                    playerName = "Unknown Player";
                }

                foreach (var table in tables)
                {
                    foreach (var record in table.Records)
                    {
                        results.Add(new
                        {
                            PlayerId = playerId,
                            PlayerName = playerName,
                            MatchId = record.GetValueByKey("match_id")?.ToString(),
                            TwoPointers = Convert.ToInt32(record.GetValueByKey("two_pointers") ?? 0),
                            ThreePointers = Convert.ToInt32(record.GetValueByKey("three_pointers") ?? 0),
                            FreeThrows = Convert.ToInt32(record.GetValueByKey("free_throws") ?? 0),
                            Assists = Convert.ToInt32(record.GetValueByKey("assists") ?? 0),
                            Fouls = Convert.ToInt32(record.GetValueByKey("fouls") ?? 0),
                            Steals = Convert.ToInt32(record.GetValueByKey("steals") ?? 0),
                            Blocks = Convert.ToInt32(record.GetValueByKey("blocks") ?? 0),
                            ReboundsOffensive = Convert.ToInt32(record.GetValueByKey("reb_off") ?? 0),
                            ReboundsDefensive = Convert.ToInt32(record.GetValueByKey("reb_def") ?? 0),
                            TotalEvents = Convert.ToInt32(record.GetValueByKey("two_pointers") ?? 0) +
                                        Convert.ToInt32(record.GetValueByKey("three_pointers") ?? 0) +
                                        Convert.ToInt32(record.GetValueByKey("free_throws") ?? 0) +
                                        Convert.ToInt32(record.GetValueByKey("assists") ?? 0) +
                                        Convert.ToInt32(record.GetValueByKey("fouls") ?? 0) +
                                        Convert.ToInt32(record.GetValueByKey("steals") ?? 0) +
                                        Convert.ToInt32(record.GetValueByKey("blocks") ?? 0) +
                                        Convert.ToInt32(record.GetValueByKey("reb_off") ?? 0) +
                                        Convert.ToInt32(record.GetValueByKey("reb_def") ?? 0)
                        });
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get player event counts for {PlayerId}", playerId);
                return new List<dynamic>();
            }
        }

        // INFLUX REPORT 3: Za zadati vremenski period (teamId = 1) vratiti za svakog igrača broj odigranih utakmica, avgPoints, avgAssists, avgFouls
        public async Task<IEnumerable<dynamic>> GetTeamPlayerAveragesInfluxReportAsync(DateTime startTime, DateTime endTime, string? teamId = "1")
        {
            try
            {
                var startTimeRfc = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                var endTimeRfc = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                
                var teamFilter = !string.IsNullOrEmpty(teamId) ? $@"|> filter(fn: (r) => r.team_id == ""{teamId}"")" : "";

                var flux = $@"
                    import ""experimental""
                    
                    // Get per-match per-player stats
                    data = from(bucket: ""{_bucket}"")
                      |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                      |> filter(fn: (r) => r._measurement == ""basketball_events"")
                      |> filter(fn: (r) => r.event_category == ""personal"")
                      |> filter(fn: (r) => r.event_type == ""+2p"" or r.event_type == ""+3p"" or r.event_type == ""+ft"" or r.event_type == ""assist"" or r.event_type == ""foul"")
                      |> filter(fn: (r) => r._field == ""event_id"")
                      {teamFilter}
                      |> group(columns: [""player_id"", ""match_id"", ""event_type""])
                      |> count()
                      |> group()

                    // Get player names
                    playerNames = from(bucket: ""{_bucket}"")
                      |> range(start: {startTimeRfc}, stop: {endTimeRfc})
                      |> filter(fn: (r) => r._measurement == ""basketball_events"")
                      |> filter(fn: (r) => r._field == ""player_name"")
                      {teamFilter}
                      |> group(columns: [""player_id""])
                      |> first()
                      |> group()
                      |> rename(columns: {{_value: ""player_name""}})

                    // Pivot to get event types as columns per match
                    pivoted = data
                      |> pivot(rowKey: [""player_id"", ""match_id""], columnKey: [""event_type""], valueColumn: ""_value"")
                      |> map(fn: (r) => ({{
                        player_id: r.player_id,
                        match_id: r.match_id,
                        two_pointers: if exists r[""+2p""] then r[""+2p""] else 0,
                        three_pointers: if exists r[""+3p""] then r[""+3p""] else 0,
                        free_throws: if exists r[""+ft""] then r[""+ft""] else 0,
                        assists: if exists r.assist then r.assist else 0,
                        fouls: if exists r.foul then r.foul else 0
                      }}))
                      |> map(fn: (r) => ({{
                        r with
                        total_points: (r.two_pointers * 2) + (r.three_pointers * 3) + r.free_throws
                      }}))

                    // Aggregate per player across all matches
                    aggregated = pivoted
                      |> group(columns: [""player_id""])
                      |> reduce(
                        identity: {{
                          player_id: """",
                          matches_played: 0,
                          total_points_sum: 0.0,
                          total_assists_sum: 0,
                          total_fouls_sum: 0
                        }},
                        fn: (r, accumulator) => ({{
                          player_id: r.player_id,
                          matches_played: accumulator.matches_played + 1,
                          total_points_sum: accumulator.total_points_sum + float(v: r.total_points),
                          total_assists_sum: accumulator.total_assists_sum + r.assists,
                          total_fouls_sum: accumulator.total_fouls_sum + r.fouls
                        }})
                      )
                      |> map(fn: (r) => ({{
                        r with
                        avg_points: if r.matches_played > 0 then r.total_points_sum / float(v: r.matches_played) else 0.0,
                        avg_assists: if r.matches_played > 0 then float(v: r.total_assists_sum) / float(v: r.matches_played) else 0.0,
                        avg_fouls: if r.matches_played > 0 then float(v: r.total_fouls_sum) / float(v: r.matches_played) else 0.0
                      }}))
                      |> filter(fn: (r) => r.matches_played > 0)
                      |> group()

                    // Join with player names
                    result = join(
                      tables: {{aggregated: aggregated, names: playerNames}},
                      on: [""player_id""]
                    )
                    |> sort(columns: [""avg_points""], desc: true)

                    result
                ";

                var queryApi = _influxDBClient.GetQueryApi();
                var tables = await queryApi.QueryAsync(flux, _org);
                var results = new List<dynamic>();

                foreach (var table in tables)
                {
                    foreach (var record in table.Records)
                    {
                        results.Add(new
                        {
                            PlayerId = record.GetValueByKey("player_id")?.ToString(),
                            PlayerName = record.GetValueByKey("player_name")?.ToString() ?? "Unknown Player",
                            TeamId = teamId,
                            MatchesPlayed = Convert.ToInt32(record.GetValueByKey("matches_played") ?? 0),
                            AvgPoints = Math.Round(Convert.ToDouble(record.GetValueByKey("avg_points") ?? 0), 2),
                            AvgAssists = Math.Round(Convert.ToDouble(record.GetValueByKey("avg_assists") ?? 0), 2),
                            AvgFouls = Math.Round(Convert.ToDouble(record.GetValueByKey("avg_fouls") ?? 0), 2),
                            TotalPointsSum = Math.Round(Convert.ToDouble(record.GetValueByKey("total_points_sum") ?? 0), 2),
                            TotalAssistsSum = Convert.ToInt32(record.GetValueByKey("total_assists_sum") ?? 0),
                            TotalFoulsSum = Convert.ToInt32(record.GetValueByKey("total_fouls_sum") ?? 0)
                        });
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get team player averages for team {TeamId}", teamId);
                return new List<dynamic>();
            }
        }

        private string BuildRangeClause(DateTime? startTime, DateTime? endTime)
        {
            if (startTime.HasValue && endTime.HasValue)
            {
                var start = startTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                var end = endTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                return $"|> range(start: {start}, stop: {end})";
            }
            else if (startTime.HasValue)
            {
                var start = startTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                return $"|> range(start: {start})";
            }
            else
            {
                return "|> range(start: -1y)";
            }
        }

        // SAGA BACKUP METHOD - VRAĆA EVENTS PRE BRISANJA
        public async Task<List<ChronologicalEventInflux>> GetEventsByPlayerAndTeamAsync(int playerId, int teamId)
        {
            try
            {
                var flux = $@"
                    from(bucket: ""{_bucket}"")
                      |> range(start: -1y)
                      |> filter(fn: (r) => r._measurement == ""basketball_events"")
                      |> filter(fn: (r) => r.player_id == ""{playerId}"")
                      |> filter(fn: (r) => r.team_id == ""{teamId}"")
                      |> sort(columns: [""_time""])";

                var events = await ExecuteQueryAsync(flux);
                return events.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get events by player {PlayerId} and team {TeamId}", playerId, teamId);
                return new List<ChronologicalEventInflux>();
            }
        }
    }
}