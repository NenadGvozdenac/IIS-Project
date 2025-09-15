using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.UndoLastEvent
{
    public class UndoLastEventHandler : IRequestHandler<UndoLastEventCommand, Result<UndoLastEventResponse>>
    {
        private readonly IPersonalEventRepository _personalEventRepository;
        private readonly IMatchTrackingRepository _matchTrackingRepository;
        private readonly MatchDbContext _context;

        public UndoLastEventHandler(
            IPersonalEventRepository personalEventRepository,
            IMatchTrackingRepository matchTrackingRepository,
            MatchDbContext context)
        {
            _personalEventRepository = personalEventRepository;
            _matchTrackingRepository = matchTrackingRepository;
            _context = context;
        }

        public Task<Result<UndoLastEventResponse>> Handle(UndoLastEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Use transaction to ensure atomicity of event deletion and score rollback
                using var transaction = _context.Database.BeginTransaction();
                
                try
                {
                    // Find the last personal event for the team in this match
                    var lastEvent = _personalEventRepository.GetLastEventByTeamAndMatch(request.MatchId, request.TeamId);
                    
                    if (lastEvent == null)
                    {
                        return Task.FromResult(Result<UndoLastEventResponse>.Failure($"No personal events found for team {request.TeamId} in match {request.MatchId}"));
                    }

                    // Get match tracking for score rollback
                    var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
                    if (matchTracking == null)
                    {
                        return Task.FromResult(Result<UndoLastEventResponse>.Failure($"Match with id {request.MatchId} not found"));
                    }

                    int pointsRolledBack = 0;
                    
                    // Handle score rollback for scoring events
                    if (IsScoreEvent(lastEvent.Type))
                    {
                        pointsRolledBack = GetPointsForEventType(lastEvent.Type);
                        
                        if (pointsRolledBack > 0)
                        {
                            // Rollback match score
                            if (request.TeamId == 1) // Our team (Partizan)
                            {
                                matchTracking.OurPoints = Math.Max(0, (matchTracking.OurPoints ?? 0) - pointsRolledBack);
                            }
                            else // Opponent team
                            {
                                matchTracking.OpponentPoints = Math.Max(0, (matchTracking.OpponentPoints ?? 0) - pointsRolledBack);
                            }

                            matchTracking.LastUpdateTime = DateTime.UtcNow;
                            _matchTrackingRepository.Update(matchTracking);
                        }
                    }

                    // Delete the event
                    var eventDeleted = _personalEventRepository.Delete(lastEvent.IdEvent);
                    
                    if (!eventDeleted)
                    {
                        return Task.FromResult(Result<UndoLastEventResponse>.Failure($"Failed to delete event with id {lastEvent.IdEvent}"));
                    }

                    // Save all changes within transaction
                    _context.SaveChanges();
                    transaction.Commit();

                    var response = new UndoLastEventResponse
                    {
                        Success = true,
                        Message = $"Successfully undone last event: {lastEvent.Type}" + 
                                 (pointsRolledBack > 0 ? $" (rolled back {pointsRolledBack} points)" : ""),
                        UndoneEventId = lastEvent.IdEvent,
                        UndoneEventType = lastEvent.Type,
                        PointsRolledBack = pointsRolledBack > 0 ? pointsRolledBack : null,
                        MatchId = request.MatchId,
                        TeamId = request.TeamId
                    };

                    return Task.FromResult(Result<UndoLastEventResponse>.Success(response));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Task.FromResult(Result<UndoLastEventResponse>.Failure($"Error undoing last event: {ex.Message}"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<UndoLastEventResponse>.Failure($"Error undoing last event: {ex.Message}"));
            }
        }

        private bool IsScoreEvent(string? eventType)
        {
            if (string.IsNullOrEmpty(eventType))
                return false;
                
            return eventType == "+2p" || eventType == "+3p" || eventType == "+ft";
        }

        private int GetPointsForEventType(string? eventType)
        {
            if (string.IsNullOrEmpty(eventType))
                return 0;
                
            return eventType switch
            {
                "+2p" => 2,
                "+3p" => 3,
                "+ft" => 1,
                _ => 0
            };
        }
    }
}