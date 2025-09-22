using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerStatistics;

public class GetPlayerStatisticsHandler : IRequestHandler<GetPlayerStatisticsQuery, Result<GetPlayerStatisticsResponse>>
{
    private readonly IPersonalEventRepository _personalEventRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IMatchRepository _matchRepository;

    public GetPlayerStatisticsHandler(
        IPersonalEventRepository personalEventRepository,
        ITeamMemberRepository teamMemberRepository,
        IMatchRepository matchRepository)
    {
        _personalEventRepository = personalEventRepository;
        _teamMemberRepository = teamMemberRepository;
        _matchRepository = matchRepository;
    }

    public Task<Result<GetPlayerStatisticsResponse>> Handle(GetPlayerStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get player information from team members
            var playerTeamMember = _teamMemberRepository.GetById(request.PlayerId, request.TeamId);
            
            if (playerTeamMember == null)
            {
                return Task.FromResult(Result<GetPlayerStatisticsResponse>.Failure("Player not found in the specified team"));
            }

            // Get all personal events for this player in this team
            var personalEvents = _personalEventRepository.GetByPlayerId(request.TeamId, request.PlayerId).ToList();
            
            // Filter events from finished matches only
            var finishedMatchIds = new HashSet<int>();
            var allMatches = _matchRepository.GetAll();
            foreach (var match in allMatches)
            {
                if (match.MatchTracking?.TrackingStatus == "finished")
                {
                    finishedMatchIds.Add(match.IdMatch);
                }
            }

            var finishedEvents = personalEvents.Where(pe => finishedMatchIds.Contains(pe.IdMatch)).ToList();

            // Calculate statistics
            var stats = CalculatePlayerStatistics(finishedEvents, finishedMatchIds.Count);

            var result = new GetPlayerStatisticsResponse
            {
                PlayerId = playerTeamMember.IdPlayer,
                PlayerName = $"{playerTeamMember.IdPlayerNavigation?.Name} {playerTeamMember.IdPlayerNavigation?.Surname}",
                Position = playerTeamMember.IdPlayerNavigation?.IdPositionNavigation?.Name ?? "Unknown",
                JerseyNumber = playerTeamMember.JerseyNumber ?? 0,
                Performance = stats,
                PlayingStyle = DeterminePlayingStyle(stats)
            };

            return Task.FromResult(Result<GetPlayerStatisticsResponse>.Success(result));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetPlayerStatisticsResponse>.Failure($"Error calculating player statistics: {ex.Message}"));
        }
    }

    private PlayerPerformance CalculatePlayerStatistics(List<PersonalEvent> events, int totalGames)
    {
        // Two-point shots
        var twoPointMade = events.Count(e => e.Type == "+2p");
        var twoPointMissed = events.Count(e => e.Type == "2p");
        var twoPointAttempted = twoPointMade + twoPointMissed;

        // Three-point shots
        var threePointMade = events.Count(e => e.Type == "+3p");
        var threePointMissed = events.Count(e => e.Type == "3p");
        var threePointAttempted = threePointMade + threePointMissed;

        // Free throws
        var freeThrowMade = events.Count(e => e.Type == "+ft");
        var freeThrowMissed = events.Count(e => e.Type == "ft");
        var freeThrowAttempted = freeThrowMade + freeThrowMissed;

        // Rebounds
        var offensiveRebounds = events.Count(e => e.Type == "reb of");
        var defensiveRebounds = events.Count(e => e.Type == "reb def");

        // Other stats
        var assists = events.Count(e => e.Type == "assist");
        var steals = events.Count(e => e.Type == "steal");
        var blocks = events.Count(e => e.Type == "block");
        var fouls = events.Count(e => e.Type == "foul");

        // Calculate total points
        var totalPoints = (twoPointMade * 2) + (threePointMade * 3) + freeThrowMade;

        return new PlayerPerformance
        {
            TwoPoint = new ShotPerformance
            {
                Made = twoPointMade,
                Attempted = twoPointAttempted,
                Percentage = twoPointAttempted > 0 ? Math.Round((double)twoPointMade / twoPointAttempted * 100, 1) : 0
            },
            ThreePoint = new ShotPerformance
            {
                Made = threePointMade,
                Attempted = threePointAttempted,
                Percentage = threePointAttempted > 0 ? Math.Round((double)threePointMade / threePointAttempted * 100, 1) : 0
            },
            FreeThrow = new ShotPerformance
            {
                Made = freeThrowMade,
                Attempted = freeThrowAttempted,
                Percentage = freeThrowAttempted > 0 ? Math.Round((double)freeThrowMade / freeThrowAttempted * 100, 1) : 0
            },
            Rebounds = new ReboundStats
            {
                Offensive = offensiveRebounds,
                Defensive = defensiveRebounds,
                Total = offensiveRebounds + defensiveRebounds
            },
            General = new PlayerGeneralStats
            {
                TotalGames = totalGames,
                PointsAvg = totalGames > 0 ? Math.Round((double)totalPoints / totalGames, 1) : 0,
                AssistsAvg = totalGames > 0 ? Math.Round((double)assists / totalGames, 1) : 0,
                StealsAvg = totalGames > 0 ? Math.Round((double)steals / totalGames, 1) : 0,
                BlocksAvg = totalGames > 0 ? Math.Round((double)blocks / totalGames, 1) : 0,
                FoulsAvg = totalGames > 0 ? Math.Round((double)fouls / totalGames, 1) : 0
            }
        };
    }

    private string DeterminePlayingStyle(PlayerPerformance stats)
    {
        if (stats.General.AssistsAvg > 5)
            return "Playmaker/Point Guard";
        
        if (stats.ThreePoint.Attempted > stats.TwoPoint.Attempted && stats.ThreePoint.Percentage > 35)
            return "Three-Point Specialist";
        
        if (stats.General.BlocksAvg > 1 || stats.Rebounds.Total > stats.General.TotalGames * 8)
            return "Interior Presence";
        
        if (stats.General.StealsAvg > 1.5)
            return "Defensive Specialist";
        
        if (stats.General.PointsAvg > 15)
            return "Primary Scorer";
        
        return "Role Player";
    }
}