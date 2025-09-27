using MassTransit;
using match_service.src.Matches.Core.Application.Saga.Messages;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using System.Text.Json;

namespace match_service.src.Matches.Core.Application.Saga.Consumers;

public class RollbackInfluxEventsConsumer : IConsumer<RollbackInfluxEventsCommand>
{
    private readonly IChronologicalEventInfluxRepository _chronologicalEventInfluxRepository;

    public RollbackInfluxEventsConsumer(IChronologicalEventInfluxRepository chronologicalEventInfluxRepository)
    {
        _chronologicalEventInfluxRepository = chronologicalEventInfluxRepository;
    }

    public async Task Consume(ConsumeContext<RollbackInfluxEventsCommand> context)
    {
        try
        {
            Console.WriteLine($"RollbackInfluxEvents: Starting rollback for backup data: {context.Message.BackupData}");
            
            // Parse backup data to extract PlayerId and TeamId
            var backupData = context.Message.BackupData;
            if (string.IsNullOrEmpty(backupData))
            {
                Console.WriteLine($"RollbackInfluxEvents: No backup data provided, skipping rollback");
                await context.Publish<InfluxEventsRolledBackEvent>(new 
                {
                    CorrelationId = context.Message.CorrelationId
                });
                return;
            }

            // Extract PlayerId and TeamId from backup string (format: "PlayerId:X,TeamId:Y")
            var parts = backupData.Split(',');
            if (parts.Length != 2)
            {
                Console.WriteLine($"RollbackInfluxEvents: Invalid backup data format, skipping rollback");
                await context.Publish<InfluxEventsRolledBackEvent>(new 
                {
                    CorrelationId = context.Message.CorrelationId
                });
                return;
            }

            var playerId = int.Parse(parts[0].Split(':')[1]);
            var teamId = int.Parse(parts[1].Split(':')[1]);

            // Parse detailed backup data
            var influxEventCount = 0;
            
            // Extract count if available (format: "PlayerId:X,TeamId:Y,IE:Z")
            if (parts.Length >= 3)
            {
                influxEventCount = int.Parse(parts[2].Split(':')[1]);
            }

            Console.WriteLine($"RollbackInfluxEvents: Starting restoration for Player {playerId}, Team {teamId}");
            
            // PRAVI ROLLBACK - DESERIALIZUJEMO I VRAĆAMO INFLUXDB PODATKE
            int restoredChronologicalEvents = 0;
            
            try 
            {
                // JSON DESERIALIZACIJA OPCIJE ZA ENTITY FRAMEWORK
                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    IgnoreReadOnlyProperties = true
                };
                
                // Vraćamo ChronologicalEventInflux zapise iz JSON backup-a
                if (!string.IsNullOrEmpty(context.Message.ChronologicalEventsBackup))
                {
                    Console.WriteLine($"ChronologicalEventInflux JSON: {context.Message.ChronologicalEventsBackup.Substring(0, Math.Min(200, context.Message.ChronologicalEventsBackup.Length))}...");
                    
                    try 
                    {
                        var ceData = JsonSerializer.Deserialize<List<dynamic>>(context.Message.ChronologicalEventsBackup, jsonOptions);
                        if (ceData?.Any() == true)
                        {
                            foreach (var item in ceData)
                            {
                                var doc = (JsonElement)item;
                                var newCe = new ChronologicalEventInflux
                                {
                                    EventId = doc.TryGetProperty("EventId", out var eventId) ? eventId.GetInt32() : 0,
                                    Timestamp = doc.TryGetProperty("Timestamp", out var timestamp) ? timestamp.GetDateTime() : DateTime.UtcNow,
                                    CreationTime = doc.TryGetProperty("CreationTime", out var creationTime) ? creationTime.GetDateTime() : DateTime.UtcNow,
                                    
                                    // Tags
                                    MatchId = doc.TryGetProperty("MatchId", out var matchIdVal) ? matchIdVal.GetString() ?? "" : "",
                                    EventCategory = doc.TryGetProperty("EventCategory", out var eventCategoryVal) ? eventCategoryVal.GetString() ?? "" : "",
                                    EventType = doc.TryGetProperty("EventType", out var eventTypeVal) ? eventTypeVal.GetString() ?? "" : "",
                                    Period = doc.TryGetProperty("Period", out var periodVal) ? periodVal.GetString() ?? "" : "",
                                    TeamId = doc.TryGetProperty("TeamId", out var teamIdVal) ? teamIdVal.GetString() : null,
                                    PlayerId = doc.TryGetProperty("PlayerId", out var playerIdVal) ? playerIdVal.GetString() : null,
                                    
                                    // Fields
                                    PlayerName = doc.TryGetProperty("PlayerName", out var playerName) ? playerName.GetString() : null,
                                    PeriodTime = doc.TryGetProperty("PeriodTime", out var periodTime) ? periodTime.GetInt32() : null,
                                    Notes = doc.TryGetProperty("Notes", out var notes) ? notes.GetString() : null,
                                    OurPoints = doc.TryGetProperty("OurPoints", out var ourPoints) ? ourPoints.GetInt32() : null,
                                    OpponentPoints = doc.TryGetProperty("OpponentPoints", out var opponentPoints) ? opponentPoints.GetInt32() : null,
                                    PointDifference = doc.TryGetProperty("PointDifference", out var pointDifference) ? pointDifference.GetInt32() : null
                                };
                                await _chronologicalEventInfluxRepository.WriteEventAsync(newCe);
                                restoredChronologicalEvents++;
                            }
                            Console.WriteLine($"RESTORED {restoredChronologicalEvents} ChronologicalEventInflux records from JSON backup");
                        }
                    }
                    catch (Exception jsonEx)
                    {
                        Console.WriteLine($"ChronologicalEventInflux JSON parse error: {jsonEx.Message}");
                    }
                }
                
                Console.WriteLine($"INFLUXDB ROLLBACK COMPLETED: Total restored {restoredChronologicalEvents} records");
            }
            catch (Exception rollbackEx)
            {
                Console.WriteLine($"INFLUXDB ROLLBACK ERROR: {rollbackEx.Message}");
                throw; // Re-throw da saga zna da je rollback neuspešan
            }
            
            await context.Publish<InfluxEventsRolledBackEvent>(new 
            {
                CorrelationId = context.Message.CorrelationId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RollbackInfluxEvents: Error during rollback - {ex.Message}");
            // U slučaju greške i dalje šaljemo success da saga može da završi
            await context.Publish<InfluxEventsRolledBackEvent>(new 
            {
                CorrelationId = context.Message.CorrelationId
            });
        }
    }
}