using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.Match.GetAllMatches;

public class GetAllMatchesHandler : IRequestHandler<GetAllMatchesQuery, Result<GetAllMatchesResponse>>
{
    private readonly IMatchRepository _matchRepository;

    public GetAllMatchesHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public Task<Result<GetAllMatchesResponse>> Handle(GetAllMatchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matches = _matchRepository.GetAll();

            var response = new GetAllMatchesResponse
            {
                Matches = matches.Select(m => new MatchDto
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
                }).ToList(),
            };

            return Task.FromResult(Result<GetAllMatchesResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllMatchesResponse>.Failure($"Failed to get matches: {ex.Message}"));
        }
    }
}
