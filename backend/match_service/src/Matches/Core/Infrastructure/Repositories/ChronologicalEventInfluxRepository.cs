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
                  |> filter(fn: (r) => r.event_type == ""+2p"" or r.event_type == ""+3p"" or r.event_type == ""assist"" or r.event_type == ""foul"")
                  |> group(columns: [""period"", ""event_type""])
                  |> aggregateWindow(every: inf, fn: count, createEmpty: false)
                  |> group(columns: [""period""])
                  |> sort(columns: [""period""])";

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
                  |> filter(fn: (r) => r.event_type == ""+2p"" or r.event_type == ""+3p"" or r.event_type == ""+ft"")
                  |> group(columns: [""player_id"", ""event_type""])
                  |> aggregateWindow(every: inf, fn: count, createEmpty: false)
                  |> group(columns: [""player_id""])
                  |> sum()
                  |> group()
                  |> sort(columns: [""_value""], desc: true)";

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

        // SLOŽEN UPIT 3: Vremenska analiza sa sliding window agregacijom
        // Trend scoringa po 5-minutnim intervalima sa moving average
        public async Task<IEnumerable<dynamic>> GetPeriodScoringTrendsAsync(string matchId)
        {
            var flux = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1y)
                  |> filter(fn: (r) => r._measurement == ""basketball_events"")
                  |> filter(fn: (r) => r.match_id == ""{matchId}"")
                  |> filter(fn: (r) => r.event_category == ""personal"")
                  |> filter(fn: (r) => r.event_type == ""+2p"" or r.event_type == ""+3p"" or r.event_type == ""+ft"")
                  |> group(columns: [""team_id""])
                  |> aggregateWindow(every: 5m, fn: count, createEmpty: true)
                  |> fill(column: ""_value"", value: 0)
                  |> movingAverage(n: 3)
                  |> group()
                  |> sort(columns: [""_time""])";

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
                            TeamId = record.GetValueByKey("team_id"),
                            TimeWindow = record.GetTime(),
                            ScoringRate = Convert.ToDouble(record.GetValue()),
                            MovingAverage = Convert.ToDouble(record.GetValue())
                        });
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get period scoring trends for {MatchId}", matchId);
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
    }
}