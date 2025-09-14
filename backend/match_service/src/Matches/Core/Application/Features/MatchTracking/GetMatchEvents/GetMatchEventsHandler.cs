using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.GetMatchEvents
{
    public class GetMatchEventsHandler : IRequestHandler<GetMatchEventsCommand, Result<GetMatchEventsResponse>>
    {
        private readonly IPersonalEventRepository _personalEventRepository;
        private readonly ITeamEventRepository _teamEventRepository;
        private readonly IGeneralEventRepository _generalEventRepository;

        public GetMatchEventsHandler(
            IPersonalEventRepository personalEventRepository,
            ITeamEventRepository teamEventRepository,
            IGeneralEventRepository generalEventRepository)
        {
            _personalEventRepository = personalEventRepository;
            _teamEventRepository = teamEventRepository;
            _generalEventRepository = generalEventRepository;
        }

        public Task<Result<GetMatchEventsResponse>> Handle(GetMatchEventsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get all events for the match
                var personalEvents = _personalEventRepository.GetByMatchId(request.MatchId);
                var teamEvents = _teamEventRepository.GetByMatchId(request.MatchId);
                var generalEvents = _generalEventRepository.GetByMatchId(request.MatchId);

                // Convert to unified DTOs
                var allEvents = new List<UnifiedEventDto>();

                // Add personal events
                foreach (var pe in personalEvents)
                {
                    allEvents.Add(new UnifiedEventDto
                    {
                        Id = pe.IdEvent,
                        EventType = "personal",
                        Type = pe.Type ?? string.Empty,
                        CreationTime = pe.CreationTime,
                        Period = pe.Period,
                        PeriodTime = pe.PeriodTime,
                        Notes = pe.Notes,
                        PlayerId = pe.IdPlayer,
                        PlayerName = pe.Id?.IdPlayerNavigation?.Name + " " + pe.Id?.IdPlayerNavigation?.Surname,
                        TeamId = pe.IdTeam
                    });
                }

                // Add team events
                foreach (var te in teamEvents)
                {
                    allEvents.Add(new UnifiedEventDto
                    {
                        Id = te.IdEvent,
                        EventType = "team",
                        Type = te.Type ?? string.Empty,
                        CreationTime = te.CreationTime,
                        Period = te.Period,
                        PeriodTime = te.PeriodTime,
                        Notes = te.Notes,
                        TeamId = te.IdTeam,
                        TeamName = te.IdTeamNavigation?.Name
                    });
                }

                // Add general events
                foreach (var ge in generalEvents)
                {
                    allEvents.Add(new UnifiedEventDto
                    {
                        Id = ge.IdEvent,
                        EventType = "general",
                        Type = ge.Type ?? string.Empty,
                        CreationTime = ge.CreationTime,
                        Period = ge.Period,
                        PeriodTime = ge.PeriodTime,
                        Notes = ge.Notes
                    });
                }

                // Sort by creation time (chronological order)
                var sortedEvents = allEvents.OrderBy(e => e.CreationTime).ToList();

                var response = new GetMatchEventsResponse
                {
                    Events = sortedEvents
                };

                return Task.FromResult(Result<GetMatchEventsResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<GetMatchEventsResponse>.Failure($"Error retrieving match events: {ex.Message}"));
            }
        }
    }
}