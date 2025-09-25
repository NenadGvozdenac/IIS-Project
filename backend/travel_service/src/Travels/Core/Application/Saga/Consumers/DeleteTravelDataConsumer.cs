using MassTransit;
using travel_service.src.Travels.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace travel_service.src.Travels.Core.Application.Saga.Consumers;

public class DeleteTravelDataConsumer : IConsumer<DeleteTravelDataCommand>
{
    private readonly ITravelInfoRepository _travelInfoRepository;
    private readonly IVisaRepository _visaRepository;
    private readonly IRequestsRepository _requestsRepository;

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
        try
        {
            var playerId = context.Message.PlayerId;
            var teamId = context.Message.TeamId;

            // Get all travel information for this player and team
            var travelInfos = _travelInfoRepository.GetByPlayerAndTeam(playerId, teamId);
            var travelInfoIds = travelInfos.Select(ti => ti.IdTravelInformation).ToList();

            // Delete visas for these travel informations
            var visas = _visaRepository.GetByTravelInformationIds(travelInfoIds);
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

            await context.Publish(new TravelDataDeletedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = playerId,
                TeamId = teamId,
                TravelInformationDeleted = travelInfosDeleted,
                VisasDeleted = visasDeleted,
                TeamMemberRequestsDeleted = teamMemberRequestsDeleted
            });
        }
        catch (Exception ex)
        {
            await context.Publish(new TravelDataDeleteFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = context.Message.PlayerId,
                TeamId = context.Message.TeamId,
                ErrorMessage = ex.Message
            });
        }
    }
}