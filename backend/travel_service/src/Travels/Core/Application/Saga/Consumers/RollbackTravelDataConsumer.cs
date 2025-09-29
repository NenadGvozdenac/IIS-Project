using MassTransit;
using match_service.src.Matches.Core.Application.Saga.Messages;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;
using System.Text.Json;

namespace travel_service.src.Travels.Core.Application.Saga.Consumers;

public class RollbackTravelDataConsumer : IConsumer<RollbackTravelDataCommand>
{
    private readonly ITravelInfoRepository _travelInfoRepository;
    private readonly IVisaRepository _visaRepository;
    private readonly IRequestsRepository _requestsRepository;
    private readonly TravelDbContext _travelDbContext;

    public RollbackTravelDataConsumer(
        ITravelInfoRepository travelInfoRepository,
        IVisaRepository visaRepository,
        IRequestsRepository requestsRepository,
        TravelDbContext travelDbContext)
    {
        _travelInfoRepository = travelInfoRepository;
        _visaRepository = visaRepository;
        _requestsRepository = requestsRepository;
        _travelDbContext = travelDbContext;
    }

    public async Task Consume(ConsumeContext<RollbackTravelDataCommand> context)
    {
        try
        {
            Console.WriteLine($"RollbackTravelData: Starting rollback for backup data: {context.Message.BackupData}");
            
            // Parse backup data to extract PlayerId and TeamId
            var backupData = context.Message.BackupData;
            if (string.IsNullOrEmpty(backupData))
            {
                Console.WriteLine($"RollbackTravelData: No backup data provided, skipping rollback");
                await context.Publish<TravelDataRolledBackEvent>(new 
                {
                    CorrelationId = context.Message.CorrelationId
                });
                return;
            }

            // Extract PlayerId and TeamId from backup string (format: "PlayerId:X,TeamId:Y")
            var parts = backupData.Split(',');
            if (parts.Length != 2)
            {
                Console.WriteLine($"RollbackTravelData: Invalid backup data format, skipping rollback");
                await context.Publish<TravelDataRolledBackEvent>(new 
                {
                    CorrelationId = context.Message.CorrelationId
                });
                return;
            }

            var playerId = int.Parse(parts[0].Split(':')[1]);
            var teamId = int.Parse(parts[1].Split(':')[1]);

            Console.WriteLine($"RollbackTravelData: Starting restoration for Player {playerId}, Team {teamId}");
            
            // PRAVI ROLLBACK - DESERIALIZUJEMO I VRAĆAMO PODATKE IZ JSON BACKUP-A
            int restoredTravelInfos = 0;
            int restoredTeamMemberRequests = 0; // VRAĆAMO SAMO TEAM MEMBER REQUEST VEZE!
            
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
                
                // Vraćamo TravelInformation zapise iz JSON backup-a
                if (!string.IsNullOrEmpty(context.Message.TravelInformationBackup))
                {
                    Console.WriteLine($"TravelInformation JSON: {context.Message.TravelInformationBackup.Substring(0, Math.Min(200, context.Message.TravelInformationBackup.Length))}...");
                    
                    try 
                    {
                        var tiData = JsonSerializer.Deserialize<List<dynamic>>(context.Message.TravelInformationBackup, jsonOptions);
                        if (tiData?.Any() == true)
                        {
                            foreach (var item in tiData)
                            {
                                var doc = (JsonElement)item;
                                
                                var idTeamValue = doc.TryGetProperty("IdTeam", out var idTeam) && idTeam.ValueKind != JsonValueKind.Null ? idTeam.GetInt32() : (int?)null;
                                var idPlayerValue = doc.TryGetProperty("IdPlayer", out var idPlayer) && idPlayer.ValueKind != JsonValueKind.Null ? idPlayer.GetInt32() : (int?)null;
                                
                                // Proverava da li već postoji TravelInformation sa istom (IdTeam, IdPlayer) kombinacijom
                                if (idTeamValue.HasValue && idPlayerValue.HasValue)
                                {
                                    var existing = _travelInfoRepository.GetByPlayerAndTeam(idPlayerValue.Value, idTeamValue.Value);
                                    if (existing?.Any() == true)
                                    {
                                        Console.WriteLine($"TravelInformation already exists for Player {idPlayerValue}, Team {idTeamValue} - skipping restore");
                                        continue;
                                    }
                                }
                                
                                var newTi = new TravelInformation
                                {
                                    PassportNumber = doc.TryGetProperty("PassportNumber", out var passport) ? passport.GetString() : null,
                                    PassportExpirationDate = doc.TryGetProperty("PassportExpirationDate", out var passportExp) ? DateOnly.FromDateTime(passportExp.GetDateTime()) : null,
                                    Phone = doc.TryGetProperty("Phone", out var phone) ? phone.GetString() : null,
                                    Email = doc.TryGetProperty("Email", out var email) ? email.GetString() : null,
                                    Role = doc.TryGetProperty("Role", out var role) ? role.GetString() : null,
                                    IdTeam = idTeamValue,
                                    IdPlayer = idPlayerValue,
                                    IdManagementMember = doc.TryGetProperty("IdManagementMember", out var idMgmt) && idMgmt.ValueKind != JsonValueKind.Null ? idMgmt.GetInt32() : null
                                };
                                
                                try
                                {
                                    _travelInfoRepository.Create(newTi);
                                    restoredTravelInfos++;
                                }
                                catch (Exception createEx)
                                {
                                    Console.WriteLine($"Failed to restore TravelInformation for Player {idPlayerValue}, Team {idTeamValue}: {createEx.Message}");
                                }
                            }
                            Console.WriteLine($"RESTORED {restoredTravelInfos} TravelInformation records from JSON backup");
                        }
                    }
                    catch (Exception jsonEx)
                    {
                        Console.WriteLine($"TravelInformation JSON parse error: {jsonEx.Message}");
                    }
                }
                
                // KLJUČNO: Vraćamo SAMO TeamMemberRequest zapise - ostali entiteti (Visa, Request) se ne vraćaju jer se ne brišu!
                if (!string.IsNullOrEmpty(context.Message.TeamMemberRequestsBackup))
                {
                    Console.WriteLine($"🔍 TeamMemberRequests JSON: {context.Message.TeamMemberRequestsBackup.Substring(0, Math.Min(200, context.Message.TeamMemberRequestsBackup.Length))}...");
                    
                    try 
                    {
                        // DESERIALIZUJEMO KAO TUPLE JER SE TAKO I ČUVA U DeleteTravelDataConsumer
                        // DESERIALIZUJEMO DTO KLASU UMESTO TUPLE-A
                        var teamMemberRequestDtos = JsonSerializer.Deserialize<List<TeamMemberRequestDto>>(context.Message.TeamMemberRequestsBackup, jsonOptions);
                        if (teamMemberRequestDtos?.Any() == true)
                        {
                            // PROVERAVAMO DA LI Request ENTITET POSTOJI PRE VRAĆANJA TeamMemberRequest VEZE
                            var validRequests = new List<(int IdTeam, int IdPlayer, int IdRequest)>();
                            
                            foreach (var dto in teamMemberRequestDtos)
                            {
                                // PROVERAVAMO DA LI Request ENTITET POSTOJI U BAZI PRE VRAĆANJA TeamMemberRequest VEZE
                                var existingRequest = _travelDbContext.Requests.Find(dto.IdRequest);
                                if (existingRequest != null)
                                {
                                    validRequests.Add((dto.IdTeam, dto.IdPlayer, dto.IdRequest));
                                    Console.WriteLine($"Request ID {dto.IdRequest} exists - will restore TeamMemberRequest ({dto.IdTeam}, {dto.IdPlayer}, {dto.IdRequest})");
                                }
                                else
                                {
                                    Console.WriteLine($"Skipping TeamMemberRequest for non-existent Request ID {dto.IdRequest}");
                                }
                            }
                            
                            if (validRequests.Any())
                            {
                                _requestsRepository.RestoreTeamMemberRequests(validRequests);
                                restoredTeamMemberRequests = validRequests.Count;
                                Console.WriteLine($"RESTORED {restoredTeamMemberRequests} TeamMemberRequest records from JSON backup");
                            }
                            else
                            {
                                Console.WriteLine($"No valid Requests found - skipping TeamMemberRequest restore");
                            }
                        }
                    }
                    catch (Exception jsonEx)
                    {
                        Console.WriteLine($"TeamMemberRequests JSON parse error: {jsonEx.Message}");
                    }
                }

                Console.WriteLine($"TRAVEL ROLLBACK COMPLETED: Total restored {restoredTravelInfos + restoredTeamMemberRequests} records");
            }
            catch (Exception rollbackEx)
            {
                Console.WriteLine($"TRAVEL ROLLBACK ERROR: {rollbackEx.Message}");
                throw; // Re-throw da saga zna da je rollback neuspešan
            }
            
            await context.Publish<TravelDataRolledBackEvent>(new 
            {
                CorrelationId = context.Message.CorrelationId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RollbackTravelData: Error during rollback - {ex.Message}");
            // U slučaju greške i dalje šaljemo success da saga može da završi
            await context.Publish<TravelDataRolledBackEvent>(new 
            {
                CorrelationId = context.Message.CorrelationId
            });
        }
    }
}