using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Application.Utilities;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEvent
{
    public class CreateEventHandler : IRequestHandler<CreateEventCommand, Result<CreateEventResponse>>
    {
        private readonly IPersonalEventRepository _personalEventRepository;
        private readonly ITeamEventRepository _teamEventRepository;
        private readonly IGeneralEventRepository _generalEventRepository;
        private readonly IMatchTrackingRepository _matchTrackingRepository;

        public CreateEventHandler(
            IPersonalEventRepository personalEventRepository,
            ITeamEventRepository teamEventRepository,
            IGeneralEventRepository generalEventRepository,
            IMatchTrackingRepository matchTrackingRepository)
        {
            _personalEventRepository = personalEventRepository;
            _teamEventRepository = teamEventRepository;
            _generalEventRepository = generalEventRepository;
            _matchTrackingRepository = matchTrackingRepository;
        }

        public Task<Result<CreateEventResponse>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate category
                if (!IsValidCategory(request.Category))
                {
                    return Task.FromResult(Result<CreateEventResponse>.Failure("Invalid category. Supported categories: personal, team, general"));
                }

                // Validate type for category
                if (!IsValidTypeForCategory(request.Category, request.Type))
                {
                    return Task.FromResult(Result<CreateEventResponse>.Failure($"Invalid type '{request.Type}' for category '{request.Category}'"));
                }

                int eventId = 0;
                string message = "";

                // prepare holders for created entities
                PersonalEvent? createdPersonal = null;
                TeamEvent? createdTeam = null;
                GeneralEvent? createdGeneral = null;

                // Fetch match and derive current period / period time from MatchTracking
                var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
                if (matchTracking == null)
                {
                    return Task.FromResult(Result<CreateEventResponse>.Failure($"Match with id {request.MatchId} not found"));
                }

                string? currentPeriod = matchTracking?.CurrentPeriod;
                int? remainingPeriodTime = matchTracking != null ? PeriodTimeCalculator.CalculateRemainingPeriodTime(matchTracking) : null;
                switch (request.Category.ToLower())
                {
                    case "personal":
                        if (request.PlayerId == null || request.TeamId == null)
                        {
                            return Task.FromResult(Result<CreateEventResponse>.Failure("PlayerId and TeamId are required for personal events"));
                        }

                        var personalEvent = new PersonalEvent
                        {
                            CreationTime = DateTime.UtcNow,
                            Notes = request.Notes,
                            Type = request.Type,
                            // use current tracking values
                            Period = currentPeriod,
                            PeriodTime = remainingPeriodTime,
                            IdTeam = request.TeamId.Value,
                            IdPlayer = request.PlayerId.Value,
                            IdMatch = request.MatchId
                        };
                        
                        _personalEventRepository.Create(personalEvent);
                        createdPersonal = personalEvent;
                        eventId = personalEvent.IdEvent;
                        message = "Personal event created successfully";
                        break;

                    case "team":
                        if (request.TeamId == null)
                        {
                            return Task.FromResult(Result<CreateEventResponse>.Failure("TeamId is required for team events"));
                        }

                        var teamEvent = new TeamEvent
                        {
                            CreationTime = DateTime.UtcNow,
                            Notes = request.Notes,
                            Type = request.Type,
                            Period = currentPeriod,
                            PeriodTime = remainingPeriodTime,
                            IdTeam = request.TeamId.Value,
                            IdMatch = request.MatchId
                        };
                        
                        _teamEventRepository.Create(teamEvent);
                        createdTeam = teamEvent;
                        eventId = teamEvent.IdEvent;
                        message = "Team event created successfully";
                        break;

                    case "general":
                        var generalEvent = new GeneralEvent
                        {
                            CreationTime = DateTime.UtcNow,
                            Notes = request.Notes,
                            Type = request.Type,
                            Period = currentPeriod,
                            PeriodTime = remainingPeriodTime,
                            IdMatch = request.MatchId
                        };
                        
                        _generalEventRepository.Create(generalEvent);
                        createdGeneral = generalEvent;
                        eventId = generalEvent.IdEvent;
                        message = "General event created successfully";
                        break;
                }

                // Compose response using created entity if available
                var response = new CreateEventResponse
                {
                    EventId = eventId,
                    Category = request.Category,
                    Type = request.Type,
                    Message = message,
                    Period = currentPeriod,
                    PeriodTime = remainingPeriodTime,
                    IdMatch = request.MatchId,
                    Notes = request.Notes,
                    CreationTime = createdPersonal?.CreationTime ?? createdTeam?.CreationTime ?? createdGeneral?.CreationTime ?? DateTime.UtcNow,
                    IdTeam = createdPersonal?.IdTeam ?? createdTeam?.IdTeam,
                    IdPlayer = createdPersonal?.IdPlayer
                };

                return Task.FromResult(Result<CreateEventResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<CreateEventResponse>.Failure($"Error creating event: {ex.Message}"));
            }
        }

        private bool IsValidCategory(string category)
        {
            var validCategories = new[] { "personal", "team", "general" };
            return validCategories.Contains(category.ToLower());
        }

        private bool IsValidTypeForCategory(string category, string type)
        {
            return category.ToLower() switch
            {
                "personal" => new[] { "+2p", "+3p", "+ft", "2p", "3p", "assist", "block", "foul", "ft", "other", "reb def", "reb of", "steal", "substitution in", "substitution out" }.Contains(type),
                "team" => new[] { "other", "technical foul", "timeout", "formation", "defense" }.Contains(type),
                "general" => new[] { "break end", "break start", "other", "pause end", "pause start", "period end", "period start" }.Contains(type),
                _ => false
            };
        }
    }
}