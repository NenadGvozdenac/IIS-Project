using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;

namespace match_service.src.Matches.Core.Application.Features.Match.StartMatch;

public class StartMatchHandler : IRequestHandler<StartMatchCommand, Result<StartMatchResponse>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;
    private readonly MatchDbContext _context;

    public StartMatchHandler(
        IMatchRepository matchRepository, 
        IMatchTrackingRepository matchTrackingRepository,
        ITeamMemberMatchRepository teamMemberMatchRepository,
        MatchDbContext context)
    {
        _matchRepository = matchRepository;
        _matchTrackingRepository = matchTrackingRepository;
        _teamMemberMatchRepository = teamMemberMatchRepository;
        _context = context;
    }

    public async Task<Result<StartMatchResponse>> Handle(StartMatchCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Validate minimum players
            if (request.OurTeamPlayerIds.Count < 5 || request.OpponentTeamPlayerIds.Count < 5)
            {
                return Result<StartMatchResponse>.Failure("Both teams must have at least 5 selected players");
            }

            // Get match and validate it exists
            var match = _matchRepository.GetById(request.MatchId);
            if (match == null)
            {
                return Result<StartMatchResponse>.Failure("Match not found");
            }

            // Get match tracking and validate it's in correct status
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Result<StartMatchResponse>.Failure("Match tracking not found");
            }

            if (matchTracking.TrackingStatus != "upcoming")
            {
                return Result<StartMatchResponse>.Failure($"Cannot start match. Current status: {matchTracking.TrackingStatus}");
            }

            // Create TeamMemberMatch entries for our team (id_team = 1)
            var teamMemberMatches = new List<Domain.Entities.TeamMemberMatch>();
            
            foreach (var playerId in request.OurTeamPlayerIds)
            {
                var teamMemberMatch = new Domain.Entities.TeamMemberMatch
                {
                    IdMatch = request.MatchId,
                    IdTeam = 1, // Our team (Partizan)
                    IdPlayer = playerId,
                    StartingLineup = true, // For now, all selected players are in starting lineup
                    InGame = false // Initially not in game
                };
                teamMemberMatches.Add(teamMemberMatch);
            }

            // Create TeamMemberMatch entries for opponent team
            foreach (var playerId in request.OpponentTeamPlayerIds)
            {
                var teamMemberMatch = new Domain.Entities.TeamMemberMatch
                {
                    IdMatch = request.MatchId,
                    IdTeam = match.IdTeam, // Opponent team
                    IdPlayer = playerId,
                    StartingLineup = true,
                    InGame = false
                };
                teamMemberMatches.Add(teamMemberMatch);
            }

            // Add all TeamMemberMatch entries
            _teamMemberMatchRepository.AddRange(teamMemberMatches);

            // Update match tracking status to preparation
            matchTracking.TrackingStatus = "preparation";
            matchTracking.LastUpdateTime = DateTime.UtcNow;
            
            _matchTrackingRepository.Update(matchTracking);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new StartMatchResponse
            {
                MatchId = request.MatchId,
                TrackingStatus = "preparation",
                CreatedTeamMemberMatches = teamMemberMatches.Count,
                Message = "Match preparation started successfully"
            };

            return Result<StartMatchResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<StartMatchResponse>.Failure($"Failed to start match: {ex.Message}");
        }
    }
}
