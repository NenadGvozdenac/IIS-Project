using MassTransit;
using System.Text.Json;
using travel_service.src.Travels.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace travel_service.src.Travels.Core.Application.Saga.Consumers;

// POMOCNA KLASA ZA JSON SERIALIZATION TUPLE-A
public class TeamMemberRequestDto
{
    public int IdTeam { get; set; }
    public int IdPlayer { get; set; }
    public int IdRequest { get; set; }
}

public class DeleteTravelDataConsumer : IConsumer<DeleteTravelDataCommand>
{
    private readonly ITravelInfoRepository _travelInfoRepository;
    private readonly IVisaRepository _visaRepository;
    private readonly IRequestsRepository _requestsRepository;
    
    // JSON serijalizacija options za Entity Framework
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    public DeleteTravelDataConsumer(
        ITravelInfoRepository travelInfoRepository,
        IVisaRepository visaRepository,
        IRequestsRepository requestsRepository)
    {
        _travelInfoRepository = travelInfoRepository;
        _visaRepository = visaRepository;
        _requestsRepository = requestsRepository;
    }

    public async Task Consume(ConsumeContext<DeleteTravelDataCommand> context)
    {
        var playerId = context.Message.PlayerId;
        var teamId = context.Message.TeamId;
        
        // BACKUP PODATAKA PRE BRISANJA - IZDVAJAMO VAN TRY-CATCH-A
        var travelInfos = _travelInfoRepository.GetByPlayerAndTeam(playerId, teamId);
        var travelInfoIds = travelInfos.Select(ti => ti.IdTravelInformation).ToList();
        var visas = _visaRepository.GetByTravelInformationIds(travelInfoIds);
        var teamMemberRequests = _requestsRepository.GetTeamMemberRequestsByPlayerAndTeam(playerId, teamId);
        
        // Serialize backup data kao JSON sa ReferenceHandler.IgnoreCycles
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };
        
        var travelInfosJson = System.Text.Json.JsonSerializer.Serialize(travelInfos.ToList(), jsonOptions);
        var visasJson = System.Text.Json.JsonSerializer.Serialize(visas.ToList(), jsonOptions);
        
        // KONVERTUJEMO TUPLE U DTO KLASU ZA PROPER JSON SERIALIZATION
        var teamMemberRequestDtos = teamMemberRequests.Select(tmr => new TeamMemberRequestDto 
        { 
            IdTeam = tmr.IdTeam, 
            IdPlayer = tmr.IdPlayer, 
            IdRequest = tmr.IdRequest 
        }).ToList();
        var teamMemberRequestsJson = System.Text.Json.JsonSerializer.Serialize(teamMemberRequestDtos, jsonOptions);
        
        Console.WriteLine($"[SAGA] BACKING UP {travelInfos.Count()} TravelInfo, {visas.Count()} Visas, {teamMemberRequests.Count()} TeamMemberRequests");

        try
        {

            // Delete visas for these travel informations
            var visasDeleted = 0;
            foreach (var visa in visas)
            {
                if (_visaRepository.Delete(visa.VisaNumber))
                    visasDeleted++;
            }
            Console.WriteLine($"[SAGA] Deleted {visasDeleted} Visa records for Player {playerId}, Team {teamId}");

            // Delete travel informations
            var travelInfosDeleted = 0;
            foreach (var travelInfo in travelInfos)
            {
                if (_travelInfoRepository.Delete(travelInfo.IdTravelInformation))
                    travelInfosDeleted++;
            }
            Console.WriteLine($"[SAGA] Deleted {travelInfosDeleted} TravelInformation records for Player {playerId}, Team {teamId}");

            // Delete team member requests
            var teamMemberRequestsDeleted = _requestsRepository.DeleteTeamMemberRequests(playerId, teamId);
            Console.WriteLine($"[SAGA] Deleted TeamMemberRequests for Player {playerId}, Team {teamId}: {teamMemberRequestsDeleted}");

            // TEST: Forsira grešku za rollback testiranje
            if (context.Message.ForceError)
            {
                throw new Exception($"FORCE ERROR FOR ROLLBACK TEST - PlayerId: {playerId}, TeamId: {teamId}");
            }

            await context.Publish(new TravelDataDeletedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = playerId,
                TeamId = teamId,
                TravelInformationDeleted = travelInfosDeleted,
                VisasDeleted = visasDeleted,
                TeamMemberRequestsDeleted = teamMemberRequestsDeleted,
                
                // DODAJEMO JSON BACKUP PODATKE
                TravelInformationBackup = travelInfosJson,
                VisasBackup = visasJson,
                RequestsBackup = null, // Requests nisu potrebni za TeamMemberRequests rollback
                TeamMemberRequestsBackup = teamMemberRequestsJson
            });
        }
        catch (Exception ex)
        {            
            await context.Publish(new TravelDataDeleteFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = context.Message.PlayerId,
                TeamId = context.Message.TeamId,
                ErrorMessage = ex.Message,
                
                // KORISTIMO BACKUP PODATKE KOJI SU SAČUVANI PRE BRISANJA
                TravelInformationBackup = travelInfosJson,
                VisasBackup = visasJson,
                RequestsBackup = null, // Requests nisu potrebni za TeamMemberRequests rollback
                TeamMemberRequestsBackup = teamMemberRequestsJson
            });
        }
    }
}