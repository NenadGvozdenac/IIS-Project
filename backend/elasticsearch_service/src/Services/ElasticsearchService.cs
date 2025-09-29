using Nest;
using elasticsearch_service.src.Models;
using elasticsearch_service.src.Models.DTOs;

namespace elasticsearch_service.src.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _playersIndex;
        private readonly string _sessionsIndex;

        public ElasticsearchService(IElasticClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            _playersIndex = _configuration["Elasticsearch:IndexNames:Players"] ?? "players";
            _sessionsIndex = _configuration["Elasticsearch:IndexNames:Sessions"] ?? "sessions";
        }

        #region Players

        public async Task<bool> CreatePlayerAsync(Player player)
        {
            var response = await _client.IndexAsync(player, i => i.Index(_playersIndex).Id(player.IdPlayer));
            return response.IsValid;
        }

        public async Task<Player?> GetPlayerAsync(int id)
        {
            var response = await _client.GetAsync<Player>(id, g => g.Index(_playersIndex));
            return response.IsValid ? response.Source : null;
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync(int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Player>(s => s
                .Index(_playersIndex)
                .From(skip)
                .Size(take)
                .Query(q => q.MatchAll()));

            return response.IsValid ? response.Documents : Enumerable.Empty<Player>();
        }

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            var response = await _client.UpdateAsync<Player>(player.IdPlayer, u => u
                .Index(_playersIndex)
                .Doc(player)
                .DocAsUpsert(true));

            return response.IsValid;
        }

        public async Task<bool> DeletePlayerAsync(int id)
        {
            var response = await _client.DeleteAsync<Player>(id, d => d.Index(_playersIndex));
            return response.IsValid;
        }

        public async Task<IEnumerable<Player>> SearchPlayersAsync(string searchTerm, int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Player>(s => s
                .Index(_playersIndex)
                .From(skip)
                .Size(take)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(p => p.Name)
                            .Field(p => p.Surname)
                            .Field(p => p.FullName)
                            .Field(p => p.Nationality)
                            .Field(p => p.Position))
                        .Query(searchTerm)
                        .Type(TextQueryType.BestFields)
                        .Fuzziness(Fuzziness.Auto))));

            return response.IsValid ? response.Documents : Enumerable.Empty<Player>();
        }

        #endregion

        #region Sessions

        public async Task<bool> CreateSessionAsync(Session session)
        {
            var response = await _client.IndexAsync(session, i => i.Index(_sessionsIndex).Id(session.IdSession));
            return response.IsValid;
        }

        public async Task<Session?> GetSessionAsync(int id)
        {
            var response = await _client.GetAsync<Session>(id, g => g.Index(_sessionsIndex));
            return response.IsValid ? response.Source : null;
        }

        public async Task<IEnumerable<Session>> GetSessionsAsync(int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .From(skip)
                .Size(take)
                .Query(q => q.MatchAll()));

            return response.IsValid ? response.Documents : Enumerable.Empty<Session>();
        }

        public async Task<bool> UpdateSessionAsync(Session session)
        {
            var response = await _client.UpdateAsync<Session>(session.IdSession, u => u
                .Index(_sessionsIndex)
                .Doc(session)
                .DocAsUpsert(true));

            return response.IsValid;
        }

        public async Task<bool> DeleteSessionAsync(int id)
        {
            var response = await _client.DeleteAsync<Session>(id, d => d.Index(_sessionsIndex));
            return response.IsValid;
        }

        public async Task<IEnumerable<Session>> SearchSessionsAsync(string searchTerm, int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .From(skip)
                .Size(take)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(s => s.PlayerFullName)
                            .Field(s => s.SessionStatus)
                            .Field(s => s.SessionType))
                        .Query(searchTerm)
                        .Type(TextQueryType.BestFields)
                        .Fuzziness(Fuzziness.Auto))));

            return response.IsValid ? response.Documents : Enumerable.Empty<Session>();
        }

        public async Task<IEnumerable<Session>> GetSessionsByPlayerAsync(int playerId, int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .From(skip)
                .Size(take)
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.IdPlayer)
                        .Value(playerId))));

            return response.IsValid ? response.Documents : Enumerable.Empty<Session>();
        }

        #endregion

        #region Index Management

        public async Task<bool> CreateIndexesAsync()
        {
            var playersResult = await CreatePlayersIndexAsync();
            var sessionsResult = await CreateSessionsIndexAsync();
            return playersResult && sessionsResult;
        }

        public async Task<bool> DeleteIndexesAsync()
        {
            var playersResult = await _client.Indices.DeleteAsync(_playersIndex);
            var sessionsResult = await _client.Indices.DeleteAsync(_sessionsIndex);
            return playersResult.IsValid && sessionsResult.IsValid;
        }

        private async Task<bool> CreatePlayersIndexAsync()
        {
            var indexExists = await _client.Indices.ExistsAsync(_playersIndex);
            if (indexExists.Exists) return true;

            var response = await _client.Indices.CreateAsync(_playersIndex, c => c
                .Map<Player>(m => m.AutoMap()));

            return response.IsValid;
        }

        private async Task<bool> CreateSessionsIndexAsync()
        {
            var indexExists = await _client.Indices.ExistsAsync(_sessionsIndex);
            if (indexExists.Exists) return true;

            var response = await _client.Indices.CreateAsync(_sessionsIndex, c => c
                .Map<Session>(m => m.AutoMap()));

            return response.IsValid;
        }

        #endregion

        #region Aggregation Methods

        public async Task<IEnumerable<PlayerSessionPointsDto>> GetPlayersWithMostPointsInSessionAsync(int limit = 5)
        {
            // Dohvati sve sesije i filtriraj po PTS metrikama
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .Size(2000)
            );

            var results = new List<PlayerSessionPointsDto>();
            
            if (response.IsValid && response.Documents != null)
            {
                foreach (var session in response.Documents)
                {
                    if (session.Metrics != null)
                    {
                        var pointsMetric = session.Metrics.FirstOrDefault(m => 
                            m.MetricName == "PTS"); // Updated to use PTS instead of points
                        
                        if (pointsMetric != null && int.TryParse(pointsMetric.MetricValue, out int points))
                        {
                            results.Add(new PlayerSessionPointsDto
                            {
                                IdPlayer = session.IdPlayer,
                                PlayerFullName = session.PlayerFullName ?? "Unknown",
                                IdSession = session.IdSession,
                                Points = points
                            });
                        }
                    }
                }
            }

            return results.OrderByDescending(r => r.Points).Take(limit).ToList();
        }

        public async Task<IEnumerable<PlayerMaxPointsDto>> GetTop5PlayersWithMaxPointsLastYearAsync()
        {
            var oneYearAgo = DateTime.UtcNow.AddYears(-1);
            
            // Jednostavniji pristup - dohvati sve sesije iz poslednje godine
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .Size(2000)
                .Query(q => q
                    .DateRange(dr => dr
                        .Field(f => f.StartTime)
                        .GreaterThanOrEquals(oneYearAgo)
                    )
                )
            );

            var playerMaxPoints = new Dictionary<int, PlayerMaxPointsDto>();
            
            if (response.IsValid && response.Documents.Any())
            {
                foreach (var session in response.Documents)
                {
                    var pointsMetric = session.Metrics?.FirstOrDefault(m => 
                        m.MetricName?.Equals("PTS", StringComparison.OrdinalIgnoreCase) == true); // Updated to use PTS
                    
                    if (pointsMetric != null && int.TryParse(pointsMetric.MetricValue, out int points))
                    {
                        if (!playerMaxPoints.ContainsKey(session.IdPlayer) || 
                            playerMaxPoints[session.IdPlayer].MaxPoints < points)
                        {
                            playerMaxPoints[session.IdPlayer] = new PlayerMaxPointsDto
                            {
                                IdPlayer = session.IdPlayer,
                                PlayerFullName = session.PlayerFullName,
                                MaxPoints = points,
                                SessionDate = session.StartTime ?? DateTime.MinValue
                            };
                        }
                    }
                }
            }

            return playerMaxPoints.Values.OrderByDescending(r => r.MaxPoints).Take(5);
        }

        public async Task<IEnumerable<PlayerPlayoffMinutesDto>> GetPlayersWithMostPlayoffMinutesByNationalityAsync(string nationality, int limit = 5)
        {
            // Get all players and sessions, filter by nationality and MIN metric
            var playersResponse = await _client.SearchAsync<Player>(s => s
                .Index(_playersIndex)
                .Size(1000)
                .Query(q => q.MatchAll())
            );

            if (!playersResponse.IsValid || !playersResponse.Documents.Any())
            {
                return Enumerable.Empty<PlayerPlayoffMinutesDto>();
            }

            // Filter players by nationality in application
            var playersFromNationality = playersResponse.Documents
                .Where(p => p.Nationality?.Equals(nationality, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            if (!playersFromNationality.Any())
            {
                return Enumerable.Empty<PlayerPlayoffMinutesDto>();
            }

            var playerIds = playersFromNationality.Select(p => p.IdPlayer).ToList();

            // Get all sessions
            var sessionsResponse = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .Size(2000)
                .Query(q => q.MatchAll())
            );

            var playerTotals = new Dictionary<int, PlayerPlayoffMinutesDto>();
            
            if (sessionsResponse.IsValid && sessionsResponse.Documents.Any())
            {
                foreach (var session in sessionsResponse.Documents)
                {
                    // Check if session is for player from requested nationality
                    if (!playerIds.Contains(session.IdPlayer)) continue;

                    var minutesMetric = session.Metrics?.FirstOrDefault(m => 
                        m.MetricName?.Equals("MIN", StringComparison.OrdinalIgnoreCase) == true); // Updated to use MIN
                    
                    if (minutesMetric != null && int.TryParse(minutesMetric.MetricValue, out int minutes))
                    {
                        if (!playerTotals.ContainsKey(session.IdPlayer))
                        {
                            var player = playersFromNationality.First(p => p.IdPlayer == session.IdPlayer);
                            playerTotals[session.IdPlayer] = new PlayerPlayoffMinutesDto
                            {
                                IdPlayer = session.IdPlayer,
                                PlayerFullName = player.FullName ?? session.PlayerFullName,
                                Nationality = nationality,
                                TotalPlayoffMinutes = 0
                            };
                        }
                        
                        playerTotals[session.IdPlayer].TotalPlayoffMinutes += minutes;
                    }
                }
            }

            return playerTotals.Values.OrderByDescending(r => r.TotalPlayoffMinutes).Take(limit);
        }

        public async Task<IEnumerable<PlayerMetricSessionDto>> GetTopPlayerSessionsByMetricAsync(string playerName, string metricName, int limit = 5)
        {
            // Rebuilt from scratch to work with new structure
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .Size(2000) // Increased size to get more results
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(match => match
                                .Field("player_full_name")
                                .Query(playerName)
                                .Fuzziness(Fuzziness.Auto)
                            )
                        )
                    )
                )
            );

            var results = new List<PlayerMetricSessionDto>();
            
            if (response.IsValid && response.Documents.Any())
            {
                foreach (var session in response.Documents)
                {
                    // Find the requested metric in this session
                    var metric = session.Metrics?.FirstOrDefault(m => 
                        m.MetricName?.Equals(metricName, StringComparison.OrdinalIgnoreCase) == true);
                    
                    if (metric != null && double.TryParse(metric.MetricValue, out double value))
                    {
                        results.Add(new PlayerMetricSessionDto
                        {
                            IdPlayer = session.IdPlayer,
                            PlayerFullName = session.PlayerFullName,
                            IdSession = session.IdSession,
                            SessionDate = session.StartTime ?? DateTime.MinValue,
                            MetricName = metricName,
                            MetricValue = value
                        });
                    }
                }
            }

            return results.OrderByDescending(r => r.MetricValue).Take(limit);
        }

        public async Task<IEnumerable<MetricTypeDto>> GetAvailableQuantitativeMetricsAsync()
        {
            // Updated to use the actual metrics from your database structure
            var quantitativeMetrics = new List<MetricTypeDto>
            {
                new MetricTypeDto { MetricName = "PTS", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "FGM", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "FGA", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "FG%", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "3PM", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "3PA", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "3P%", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "FTM", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "FTA", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "FT%", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "OREB", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "DREB", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "REB", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "AST", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "STL", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "BLK", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "TOV", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "PF", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "MIN", MetricType = "Quantitative" },
                new MetricTypeDto { MetricName = "+/-", MetricType = "Quantitative" },
                // Descriptive metrics
                new MetricTypeDto { MetricName = "Leadership Qualities", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Team Chemistry", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Communication Skills", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Work Ethic", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Basketball IQ", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Defensive Intensity", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Clutch Performance", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Coachability", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Mental Toughness", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Court Vision", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Shooting Form", MetricType = "Descriptive" },
                new MetricTypeDto { MetricName = "Ball Handling Skills", MetricType = "Descriptive" }
            };

            return await Task.FromResult(quantitativeMetrics);
        }

        public async Task<bool> BulkInsertPlayersAsync(IEnumerable<Player> players)
        {
            var response = await _client.BulkAsync(b => b
                .Index(_playersIndex)
                .IndexMany(players, (descriptor, player) => descriptor.Id(player.IdPlayer))
            );

            return response.IsValid;
        }

        public async Task<bool> BulkInsertSessionsAsync(IEnumerable<Session> sessions)
        {
            var response = await _client.BulkAsync(b => b
                .Index(_sessionsIndex)
                .IndexMany(sessions, (descriptor, session) => descriptor.Id(session.IdSession))
            );

            return response.IsValid;
        }

        public async Task<bool> GenerateDummyDataAsync()
        {
            var random = new Random();
            var countries = new[] { "USA", "Serbia", "Croatia", "Greece", "Spain", "France", "Germany", "Italy", "Lithuania", "Slovenia" };
            var positions = new[] { "Point Guard", "Shooting Guard", "Small Forward", "Power Forward", "Center" };
            var firstNames = new[] { "John", "Michael", "Luka", "Nikola", "Giannis", "LeBron", "Stephen", "Kevin", "James", "Anthony", 
                                   "Damian", "Russell", "Chris", "Blake", "Paul", "Kawhi", "Joel", "Jimmy", "Kyrie", "Klay" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                                  "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };
            var sessionTypes = new[] { "Training", "Game", "Practice", "Scrimmage", "Playoff", "Regular Season" };
            var sessionStatuses = new[] { "Completed", "In Progress", "Scheduled", "Cancelled" };

            // Generate 100 players
            var players = new List<Player>();
            for (int i = 1; i <= 100; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                
                players.Add(new Player
                {
                    IdPlayer = i,
                    Name = firstName,
                    Surname = lastName,
                    FullName = $"{firstName} {lastName}",
                    Birthday = DateTime.Now.AddYears(-random.Next(18, 40)).AddDays(-random.Next(0, 365)),
                    Nationality = countries[random.Next(countries.Length)],
                    Position = positions[random.Next(positions.Length)],
                    PhysicalMetrics = new List<PhysicalMetric>
                    {
                        new PhysicalMetric
                        {
                            VerticalJump = random.Next(60, 120),
                            FatPercentage = random.Next(5, 15),
                            BenchPressWeight = random.Next(80, 150),
                            SquatWeight = random.Next(100, 200),
                            SprintSpeed = random.Next(70, 100),
                            Weight = random.Next(70, 120),
                            Height = random.Next(170, 220),
                            Wingspan = random.Next(175, 230),
                            DateOfMeasurement = DateTime.Now.AddDays(-random.Next(0, 365))
                        }
                    }
                });
            }

            // Generate 1000 sessions
            var sessions = new List<Session>();
            for (int i = 1; i <= 1000; i++)
            {
                var startTime = DateTime.Now.AddDays(-random.Next(0, 730)); // Last 2 years
                var playerId = random.Next(1, 101); // Random player from 1-100
                var player = players.FirstOrDefault(p => p.IdPlayer == playerId);

                var metrics = new List<SessionMetric>();
                
                // NBA Box Score Stats (Quantitative Metrics)
                var points = random.Next(0, 50);
                var fgm = random.Next(0, Math.Min(20, points / 2 + 3));
                var fga = Math.Max(fgm, fgm + random.Next(2, 10));
                var fgPercentage = fga > 0 ? Math.Round((double)fgm / fga * 100, 1) : 0;
                
                var threepm = random.Next(0, Math.Min(8, fgm));
                var threepa = Math.Max(threepm, threepm + random.Next(1, 6));
                var threepPercentage = threepa > 0 ? Math.Round((double)threepm / threepa * 100, 1) : 0;
                
                var ftm = random.Next(0, 12);
                var fta = Math.Max(ftm, ftm + random.Next(0, 4));
                var ftPercentage = fta > 0 ? Math.Round((double)ftm / fta * 100, 1) : 0;
                
                var oreb = random.Next(0, 8);
                var dreb = random.Next(0, 15);
                var totalReb = oreb + dreb;
                
                // Add all NBA box score metrics
                metrics.Add(new SessionMetric { MetricName = "PTS", MetricValue = points.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "FGM", MetricValue = fgm.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "FGA", MetricValue = fga.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "FG%", MetricValue = fgPercentage.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "3PM", MetricValue = threepm.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "3PA", MetricValue = threepa.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "3P%", MetricValue = threepPercentage.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "FTM", MetricValue = ftm.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "FTA", MetricValue = fta.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "FT%", MetricValue = ftPercentage.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "OREB", MetricValue = oreb.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "DREB", MetricValue = dreb.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "REB", MetricValue = totalReb.ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "AST", MetricValue = random.Next(0, 15).ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "STL", MetricValue = random.Next(0, 8).ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "BLK", MetricValue = random.Next(0, 6).ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "TOV", MetricValue = random.Next(0, 8).ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "PF", MetricValue = random.Next(0, 6).ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "MIN", MetricValue = random.Next(10, 48).ToString(), MetricType = "Quantitative" });
                metrics.Add(new SessionMetric { MetricName = "+/-", MetricValue = (random.Next(-20, 21)).ToString(), MetricType = "Quantitative" });

                // Descriptive Metrics (1-10 scale ratings) - Only add some randomly
                if (random.NextDouble() > 0.3) // 70% chance to have descriptive metrics
                {
                    var descriptiveMetrics = new[]
                    {
                        "Leadership Qualities", "Team Chemistry", "Communication Skills", "Work Ethic",
                        "Basketball IQ", "Defensive Intensity", "Clutch Performance", "Coachability",
                        "Mental Toughness", "Court Vision", "Shooting Form", "Ball Handling Skills"
                    };
                    
                    // Add 3-6 random descriptive metrics per session
                    var numDescriptive = random.Next(3, 7);
                    var selectedMetrics = descriptiveMetrics.OrderBy(x => random.Next()).Take(numDescriptive);
                    
                    foreach (var metric in selectedMetrics)
                    {
                        var rating = Math.Round(random.NextDouble() * 4 + 6, 1); // 6.0 - 10.0 scale
                        metrics.Add(new SessionMetric { MetricName = metric, MetricValue = rating.ToString(), MetricType = "Descriptive" });
                    }
                }

                sessions.Add(new Session
                {
                    IdSession = i,
                    StartTime = startTime,
                    EndTime = startTime.AddHours(random.Next(1, 4)),
                    SessionStatus = sessionStatuses[random.Next(sessionStatuses.Length)],
                    SessionType = sessionTypes[random.Next(sessionTypes.Length)],
                    IdPlayer = playerId,
                    PlayerFullName = player?.FullName ?? "Unknown Player",
                    Metrics = metrics
                });
            }

            // Insert players and sessions
            var playersInserted = await BulkInsertPlayersAsync(players);
            var sessionsInserted = await BulkInsertSessionsAsync(sessions);

            return playersInserted && sessionsInserted;
        }

        #endregion
    }
}