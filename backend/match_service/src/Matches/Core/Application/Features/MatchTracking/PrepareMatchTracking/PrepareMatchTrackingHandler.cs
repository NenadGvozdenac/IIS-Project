using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.PrepareMatchTracking
{
    public class PrepareMatchTrackingHandler : IRequestHandler<PrepareMatchTrackingCommand, Result<PrepareMatchTrackingResponse>>
    {
        private readonly IMatchTrackingRepository _matchTrackingRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;
        private readonly MatchDbContext _context;

        public PrepareMatchTrackingHandler(
            IMatchTrackingRepository matchTrackingRepository,
            IMatchRepository matchRepository,
            ITeamMemberMatchRepository teamMemberMatchRepository,
            MatchDbContext context)
        {
            _matchTrackingRepository = matchTrackingRepository;
            _matchRepository = matchRepository;
            _teamMemberMatchRepository = teamMemberMatchRepository;
            _context = context;
        }

        public Task<Result<PrepareMatchTrackingResponse>> Handle(PrepareMatchTrackingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate minimum players requirement
                if (request.OurTeamPlayerIds.Count < 5 || request.OpponentTeamPlayerIds.Count < 5)
                {
                    return Task.FromResult(Result<PrepareMatchTrackingResponse>.Failure("Each team must have at least 5 players selected"));
                }

                // Check if match exists
                var match = _matchRepository.GetById(request.MatchId);
                if (match == null)
                {
                    return Task.FromResult(Result<PrepareMatchTrackingResponse>.Failure("Match not found"));
                }

                // Check if match tracking exists
                var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
                if (matchTracking == null)
                {
                    return Task.FromResult(Result<PrepareMatchTrackingResponse>.Failure("Match tracking not found"));
                }

                // Validate that tracking status is 'upcoming'
                if (matchTracking.TrackingStatus != "upcoming")
                {
                    return Task.FromResult(Result<PrepareMatchTrackingResponse>.Failure($"Cannot prepare match. Current status: {matchTracking.TrackingStatus}"));
                }

                // Update tracking status to 'preparation'
                matchTracking.TrackingStatus = "preparation";
                matchTracking.LastUpdateTime = DateTime.UtcNow;
                
                // Set analyst ID if provided
                if (request.AnalystId.HasValue)
                {
                    matchTracking.IdUser = request.AnalystId.Value;
                }
                
                _matchTrackingRepository.Update(matchTracking);

                // Get team IDs - assuming team ID 1 is our team and the opponent team ID from match
                var ourTeamId = 1; // This should be configurable or retrieved from settings
                var opponentTeamId = match.IdTeam;

                var teamMemberMatches = new List<Domain.Entities.TeamMemberMatch>();

                // Create TeamMemberMatch entries for our team
                foreach (var playerId in request.OurTeamPlayerIds)
                {
                    var teamMemberMatch = new Domain.Entities.TeamMemberMatch
                    {
                        IdMatch = request.MatchId,
                        IdTeam = ourTeamId,
                        IdPlayer = playerId,
                        StartingLineup = false, // Can be updated later
                        InGame = false
                    };

                    teamMemberMatches.Add(teamMemberMatch);
                }

                // Create TeamMemberMatch entries for opponent team
                foreach (var playerId in request.OpponentTeamPlayerIds)
                {
                    var teamMemberMatch = new Domain.Entities.TeamMemberMatch
                    {
                        IdMatch = request.MatchId,
                        IdTeam = opponentTeamId,
                        IdPlayer = playerId,
                        StartingLineup = false, // Can be updated later
                        InGame = false
                    };

                    teamMemberMatches.Add(teamMemberMatch);
                }

                _teamMemberMatchRepository.AddRange(teamMemberMatches);
                _context.SaveChanges();

                var response = new PrepareMatchTrackingResponse
                {
                    Success = true,
                    Message = "Match preparation completed successfully",
                    MatchId = request.MatchId,
                    NewTrackingStatus = "preparation"
                };

                return Task.FromResult(Result<PrepareMatchTrackingResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<PrepareMatchTrackingResponse>.Failure($"Error preparing match: {ex.Message}"));
            }
        }
    }
}
