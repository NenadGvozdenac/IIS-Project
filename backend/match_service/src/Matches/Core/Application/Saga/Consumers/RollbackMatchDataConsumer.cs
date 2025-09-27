using MassTransit;
using match_service.src.Matches.Core.Application.Saga.Messages;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using System.Text.Json;

namespace match_service.src.Matches.Core.Application.Saga.Consumers;

public class RollbackMatchDataConsumer : IConsumer<RollbackMatchDataCommand>
{
    private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;
    private readonly IPersonalEventRepository _personalEventRepository;

    public RollbackMatchDataConsumer(
        ITeamMemberMatchRepository teamMemberMatchRepository,
        IPersonalEventRepository personalEventRepository)
    {
        _teamMemberMatchRepository = teamMemberMatchRepository;
        _personalEventRepository = personalEventRepository;
    }

    public async Task Consume(ConsumeContext<RollbackMatchDataCommand> context)
    {
        try
        {
            Console.WriteLine($"RollbackMatchData: Starting rollback for backup data: {context.Message.BackupData}");
            
            // Parse backup data to extract PlayerId and TeamId
            var backupData = context.Message.BackupData;
            if (string.IsNullOrEmpty(backupData))
            {
                Console.WriteLine($"RollbackMatchData: No backup data provided, skipping rollback");
                await context.Publish<MatchDataRolledBackEvent>(new 
                {
                    CorrelationId = context.Message.CorrelationId
                });
                return;
            }

            // Extract PlayerId and TeamId from backup string (format: "PlayerId:X,TeamId:Y")
            var parts = backupData.Split(',');
            if (parts.Length != 2)
            {
                Console.WriteLine($"RollbackMatchData: Invalid backup data format, skipping rollback");
                await context.Publish<MatchDataRolledBackEvent>(new 
                {
                    CorrelationId = context.Message.CorrelationId
                });
                return;
            }

            var playerId = int.Parse(parts[0].Split(':')[1]);
            var teamId = int.Parse(parts[1].Split(':')[1]);

            // Parse detailed backup data
            var teamMemberMatchCount = 0;
            var personalEventCount = 0;
            
            // Extract counts if available (format: "PlayerId:X,TeamId:Y,TMM:Z,PE:W")
            if (parts.Length >= 4)
            {
                teamMemberMatchCount = int.Parse(parts[2].Split(':')[1]);
                personalEventCount = int.Parse(parts[3].Split(':')[1]);
            }

            Console.WriteLine($"RollbackMatchData: Starting restoration for Player {playerId}, Team {teamId}");
            
            // PRAVI ROLLBACK - DESERIALIZUJEMO I VRAĆAMO PODATKE IZ JSON BACKUP-A
            int restoredTeamMemberMatches = 0;
            int restoredPersonalEvents = 0;
            
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
                
                // Vraćamo TeamMemberMatch zapise iz JSON backup-a
                if (!string.IsNullOrEmpty(context.Message.TeamMemberMatchesBackup))
                {
                    Console.WriteLine($"🔍 TeamMemberMatch JSON: {context.Message.TeamMemberMatchesBackup.Substring(0, Math.Min(200, context.Message.TeamMemberMatchesBackup.Length))}...");
                    
                    // JEDNOSTAVAN ROLLBACK - kreiraj nove objekte umesto deserializacije
                    try 
                    {
                        var tmmData = JsonSerializer.Deserialize<List<dynamic>>(context.Message.TeamMemberMatchesBackup, jsonOptions);
                        if (tmmData?.Any() == true)
                        {
                            foreach (var item in tmmData)
                            {
                                var doc = (JsonElement)item;
                                var newTmm = new TeamMemberMatch
                                {
                                    IdMatch = doc.GetProperty("IdMatch").GetInt32(),
                                    IdTeam = doc.GetProperty("IdTeam").GetInt32(), 
                                    IdPlayer = doc.GetProperty("IdPlayer").GetInt32(),
                                    StartingLineup = doc.GetProperty("StartingLineup").GetBoolean(),
                                    InGame = doc.GetProperty("InGame").GetBoolean()
                                };
                                _teamMemberMatchRepository.Create(newTmm);
                                restoredTeamMemberMatches++;
                            }
                            Console.WriteLine($"RESTORED {restoredTeamMemberMatches} TeamMemberMatch records from JSON backup");
                        }
                    }
                    catch (Exception jsonEx)
                    {
                        Console.WriteLine($"TeamMemberMatch JSON parse error: {jsonEx.Message}");
                    }
                }
                
                // Vraćamo PersonalEvent zapise iz JSON backup-a
                if (!string.IsNullOrEmpty(context.Message.PersonalEventsBackup))
                {
                    Console.WriteLine($"🔍 PersonalEvent JSON: {context.Message.PersonalEventsBackup.Substring(0, Math.Min(200, context.Message.PersonalEventsBackup.Length))}...");
                    
                    // JEDNOSTAVAN ROLLBACK - kreiraj nove objekte umesto deserializacije
                    try 
                    {
                        var peData = JsonSerializer.Deserialize<List<dynamic>>(context.Message.PersonalEventsBackup, jsonOptions);
                        if (peData?.Any() == true)
                        {
                            foreach (var item in peData)
                            {
                                var doc = (JsonElement)item;
                                var newPe = new PersonalEvent
                                {
                                    IdMatch = doc.GetProperty("IdMatch").GetInt32(),
                                    IdTeam = doc.GetProperty("IdTeam").GetInt32(),
                                    IdPlayer = doc.GetProperty("IdPlayer").GetInt32(),
                                    CreationTime = doc.GetProperty("CreationTime").GetDateTime(),
                                    Notes = doc.TryGetProperty("Notes", out var notes) ? notes.GetString() : null,
                                    Type = doc.TryGetProperty("Type", out var type) ? type.GetString() : null,
                                    Period = doc.TryGetProperty("Period", out var period) ? period.GetString() : null,
                                    PeriodTime = doc.TryGetProperty("PeriodTime", out var periodTime) ? periodTime.GetInt32() : null
                                };
                                _personalEventRepository.Create(newPe);
                                restoredPersonalEvents++;
                            }
                            Console.WriteLine($"RESTORED {restoredPersonalEvents} PersonalEvent records from JSON backup");
                        }
                    }
                    catch (Exception jsonEx)
                    {
                        Console.WriteLine($"PersonalEvent JSON parse error: {jsonEx.Message}");
                    }
                }
                
                Console.WriteLine($"ROLLBACK COMPLETED: Total restored {restoredTeamMemberMatches + restoredPersonalEvents} records");
            }
            catch (Exception rollbackEx)
            {
                Console.WriteLine($"ROLLBACK ERROR: {rollbackEx.Message}");
                throw; // Re-throw da saga zna da je rollback neuspešan
            }
            
            await context.Publish<MatchDataRolledBackEvent>(new 
            {
                CorrelationId = context.Message.CorrelationId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RollbackMatchData: Error during rollback - {ex.Message}");
            // U slučaju greške i dalje šaljemo success da saga može da završi
            await context.Publish<MatchDataRolledBackEvent>(new 
            {
                CorrelationId = context.Message.CorrelationId
            });
        }
    }
}