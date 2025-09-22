using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Features.Match.GetAllMatches;

namespace match_service.src.Matches.Core.Application.Features.Match.GetFinishedMatchesByTeam;

public class GetFinishedMatchesByTeamHandler : IRequestHandler<GetFinishedMatchesByTeamQuery, Result<GetFinishedMatchesByTeamResponse>>
{
    private readonly IMatchRepository _matchRepository;

    public GetFinishedMatchesByTeamHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public Task<Result<GetFinishedMatchesByTeamResponse>> Handle(GetFinishedMatchesByTeamQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var allMatches = _matchRepository.GetAll();
            List<Domain.Entities.Match> filteredMatches;
            bool isOurTeam = request.TeamId == 1; // Assuming our team always has ID = 1

            if (isOurTeam)
            {
                // For our team (ID=1), get all finished matches (we participate in all matches)
                filteredMatches = allMatches
                    .Where(m => m.MatchTracking != null && 
                               m.MatchTracking.TrackingStatus == "finished")
                    .ToList();
            }
            else
            {
                // For opponent teams, get only finished matches where this team is the opponent
                filteredMatches = allMatches
                    .Where(m => m.IdTeam == request.TeamId && 
                               m.MatchTracking != null && 
                               m.MatchTracking.TrackingStatus == "finished")
                    .ToList();
            }

            var response = new GetFinishedMatchesByTeamResponse
            {
                Matches = filteredMatches.Select(m => new MatchDto
                {
                    IdMatch = m.IdMatch,
                    Name = m.Name,
                    ScheduledAt = m.ScheduledAt,
                    Type = m.Type,
                    State = m.State,
                    City = m.City,
                    Hall = m.Hall,
                    IsInOurHall = m.IsInOurHall,
                    IdTeam = m.IdTeam,
                    TeamName = m.IdTeamNavigation?.Name,
                    CompetitionName = m.IdCompetitionNavigation?.Name,
                    SeasonName = m.IdSeasonNavigation?.Name,
                    TrackingStatus = m.MatchTracking?.TrackingStatus,
                    OurPoints = m.MatchTracking?.OurPoints,
                    OpponentPoints = m.MatchTracking?.OpponentPoints,
                    CurrentPeriod = m.MatchTracking?.CurrentPeriod,
                    StartTime = m.MatchTracking?.StartTime,
                    EndTime = m.MatchTracking?.EndTime
                })
                .OrderByDescending(m => m.ScheduledAt) // Sort by date, newest first
                .ToList()
            };

            return Task.FromResult(Result<GetFinishedMatchesByTeamResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetFinishedMatchesByTeamResponse>.Failure($"Failed to get finished matches for team {request.TeamId}: {ex.Message}"));
        }
    }
}