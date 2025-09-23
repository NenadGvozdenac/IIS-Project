using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetTeamStatistics
{
    public class GetTeamStatisticsHandler : IRequestHandler<GetTeamStatisticsQuery, Result<GetTeamStatisticsResponse>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IPersonalEventRepository _personalEventRepository;
        private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;

        public GetTeamStatisticsHandler(
            ITeamRepository teamRepository,
            IMatchRepository matchRepository,
            IPersonalEventRepository personalEventRepository,
            ITeamMemberMatchRepository teamMemberMatchRepository)
        {
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _personalEventRepository = personalEventRepository;
            _teamMemberMatchRepository = teamMemberMatchRepository;
        }

        public Task<Result<GetTeamStatisticsResponse>> Handle(GetTeamStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var team = _teamRepository.GetById(request.TeamId);
                if (team == null)
                {
                    return Task.FromResult(Result<GetTeamStatisticsResponse>.Failure($"Team with ID {request.TeamId} not found")
                        .WithCode((int)ResultCode.NotFound));
                }

                // Get matches based on team type
                List<Domain.Entities.Match> teamMatches;
                bool isOurTeam = request.TeamId == 1; // Assuming our team always has ID = 1
                
                if (isOurTeam)
                {
                    // For our team (ID=1), get all matches where match tracking exists (we participate in all matches)
                    teamMatches = _matchRepository.GetAll()
                        .Where(m => m.MatchTracking != null && m.MatchTracking.TrackingStatus == "finished")
                        .ToList();
                }
                else
                {
                    // For opponent teams, get only matches where this team is the opponent (stored in IdTeam field)
                    teamMatches = _matchRepository.GetAll()
                        .Where(m => m.IdTeam == request.TeamId && m.MatchTracking != null && 
                                    m.MatchTracking.TrackingStatus == "finished")
                        .ToList();
                }

                // Calculate statistics from personal events
                var stats = CalculateTeamStatistics(teamMatches, request.TeamId, isOurTeam);

                var response = new GetTeamStatisticsResponse
                {
                    PlayingStyle = team.PlayingStyle ?? "",
                    HeadToHead = GenerateHeadToHeadString(teamMatches, isOurTeam),
                    Performance = stats,
                    KeyStrengths = SplitAndClean(team.KeyStrengths),
                    KeyWeaknesses = SplitAndClean(team.KeyWeaknesses)
                };

                return Task.FromResult(Result<GetTeamStatisticsResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<GetTeamStatisticsResponse>.Failure($"Error retrieving team statistics: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError));
            }
        }

        private TeamPerformanceStats CalculateTeamStatistics(List<Domain.Entities.Match> matches, int teamId, bool isOurTeam)
        {
            var stats = new TeamPerformanceStats();
            
            if (!matches.Any())
            {
                return stats;
            }

            int totalPoints = 0;
            int totalAssists = 0;
            int totalSteals = 0;
            int totalBlocks = 0;
            int totalTurnovers = 0;
            int wins = 0;
            int losses = 0;

            // Two point stats
            int twoPointMade = 0, twoPointAttempted = 0;
            // Three point stats  
            int threePointMade = 0, threePointAttempted = 0;
            // Free throw stats
            int freeThrowMade = 0, freeThrowAttempted = 0;
            // Rebound stats
            int offensiveRebounds = 0, defensiveRebounds = 0;

            foreach (var match in matches)
            {
                // Get all personal events for this team in this match
                // Use repository method that filters by match and team
                var teamMembers = _teamMemberMatchRepository.GetByMatchAndTeamId(match.IdMatch, teamId);

                // TeamMemberMatch exposes the player id as IdPlayer
                var teamPlayerIds = teamMembers.Select(tm => tm.IdPlayer).ToList();

                // PersonalEvent stores the player id in IdPlayer as well
                var matchEvents = _personalEventRepository.GetByMatchId(match.IdMatch)
                    .Where(pe => teamPlayerIds.Contains(pe.IdPlayer))
                    .ToList();

                // Count different event types
                foreach (var eventItem in matchEvents)
                {
                    switch (eventItem.Type?.ToLower())
                    {
                        case "+2p":
                            twoPointMade++;
                            twoPointAttempted++;
                            totalPoints += 2;
                            break;
                        case "2p":
                            twoPointAttempted++;
                            break;
                        case "+3p":
                            threePointMade++;
                            threePointAttempted++;
                            totalPoints += 3;
                            break;
                        case "3p":
                            threePointAttempted++;
                            break;
                        case "+ft":
                            freeThrowMade++;
                            freeThrowAttempted++;
                            totalPoints += 1;
                            break;
                        case "ft":
                            freeThrowAttempted++;
                            break;
                        case "reb of":
                            offensiveRebounds++;
                            break;
                        case "reb def":
                            defensiveRebounds++;
                            break;
                        case "assist":
                            totalAssists++;
                            break;
                        case "steal":
                            totalSteals++;
                            break;
                        case "block":
                            totalBlocks++;
                            break;
                        case "foul":
                            // Could add turnover logic here if needed
                            break;
                    }
                }

                // Determine win/loss based on team perspective
                var ourScore = match.MatchTracking?.OurPoints ?? 0;
                var opponentScore = match.MatchTracking?.OpponentPoints ?? 0;
                
                if (isOurTeam)
                {
                    // For our team, OurPoints represents our score
                    if (ourScore > opponentScore)
                        wins++;
                    else if (ourScore < opponentScore)
                        losses++;
                }
                else
                {
                    // For opponent teams, OpponentPoints represents their score
                    if (opponentScore > ourScore)
                        wins++;
                    else if (opponentScore < ourScore)
                        losses++;
                }
            }

            var totalGames = matches.Count;

            // Calculate averages and percentages
            stats.TwoPoint = new ShotStats
            {
                Made = twoPointMade,
                Attempted = twoPointAttempted,
                Percentage = twoPointAttempted > 0 ? Math.Round((double)twoPointMade / twoPointAttempted * 100, 1) : 0
            };

            stats.ThreePoint = new ShotStats
            {
                Made = threePointMade,
                Attempted = threePointAttempted,
                Percentage = threePointAttempted > 0 ? Math.Round((double)threePointMade / threePointAttempted * 100, 1) : 0
            };

            stats.FreeThrow = new ShotStats
            {
                Made = freeThrowMade,
                Attempted = freeThrowAttempted,
                Percentage = freeThrowAttempted > 0 ? Math.Round((double)freeThrowMade / freeThrowAttempted * 100, 1) : 0
            };

            stats.Rebounds = new ReboundStats
            {
                Offensive = offensiveRebounds,
                Defensive = defensiveRebounds,
                Total = offensiveRebounds + defensiveRebounds
            };

            stats.General = new GeneralStats
            {
                AssistsAvg = totalGames > 0 ? Math.Round((double)totalAssists / totalGames, 1) : 0,
                TurnoversAvg = totalGames > 0 ? Math.Round((double)totalTurnovers / totalGames, 1) : 0,
                StealsAvg = totalGames > 0 ? Math.Round((double)totalSteals / totalGames, 1) : 0,
                BlocksAvg = totalGames > 0 ? Math.Round((double)totalBlocks / totalGames, 1) : 0,
                PointsAvg = totalGames > 0 ? Math.Round((double)totalPoints / totalGames, 1) : 0,
                TotalGames = totalGames,
                Wins = wins,
                Losses = losses,
                WinPercentage = totalGames > 0 ? Math.Round((double)wins / totalGames * 100, 1) : 0
            };

            return stats;
        }

        private string GenerateHeadToHeadString(List<Domain.Entities.Match> matches, bool isOurTeam)
        {
            int wins, losses, draws;
            
            if (isOurTeam)
            {
                // For our team, OurPoints vs OpponentPoints
                wins = matches.Count(m => (m.MatchTracking?.OurPoints ?? 0) > (m.MatchTracking?.OpponentPoints ?? 0));
                losses = matches.Count(m => (m.MatchTracking?.OurPoints ?? 0) < (m.MatchTracking?.OpponentPoints ?? 0));
                draws = matches.Count(m => (m.MatchTracking?.OurPoints ?? 0) == (m.MatchTracking?.OpponentPoints ?? 0));
            }
            else
            {
                // For opponent teams, OpponentPoints vs OurPoints
                wins = matches.Count(m => (m.MatchTracking?.OpponentPoints ?? 0) > (m.MatchTracking?.OurPoints ?? 0));
                losses = matches.Count(m => (m.MatchTracking?.OpponentPoints ?? 0) < (m.MatchTracking?.OurPoints ?? 0));
                draws = matches.Count(m => (m.MatchTracking?.OpponentPoints ?? 0) == (m.MatchTracking?.OurPoints ?? 0));
            }
            
            return $"{wins}-{losses}" + (draws > 0 ? $"-{draws}" : "") + $" (W-L" + (draws > 0 ? "-D" : "") + ")";
        }

        private List<string> SplitAndClean(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            return input.Split(new char[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => s.Trim())
                       .Where(s => !string.IsNullOrEmpty(s))
                       .ToList();
        }
    }
}