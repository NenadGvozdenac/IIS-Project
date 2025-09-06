using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.Match.GetMatchById;

public class GetMatchByIdHandler : IRequestHandler<GetMatchByIdQuery, Result<GetMatchByIdResponse>>
{
    private readonly IMatchRepository _matchRepository;

    public GetMatchByIdHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public Task<Result<GetMatchByIdResponse>> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var match = _matchRepository.GetById(request.Id);
            
            if (match == null)
            {
                return Task.FromResult(Result<GetMatchByIdResponse>.Failure("Match not found"));
            }

            var response = new GetMatchByIdResponse
            {
                IdMatch = match.IdMatch,
                Name = match.Name,
                ScheduledAt = match.ScheduledAt,
                Type = match.Type,
                State = match.State,
                City = match.City,
                Hall = match.Hall,
                IsInOurHall = match.IsInOurHall,
                IdCompetition = match.IdCompetition,
                IdSeason = match.IdSeason,
                IdTeam = match.IdTeam,
                TeamName = match.IdTeamNavigation?.Name,
                CompetitionName = match.IdCompetitionNavigation?.Name,
                SeasonName = match.IdSeasonNavigation?.Name,
                TrackingStatus = match.MatchTracking?.TrackingStatus,
                OurPoints = match.MatchTracking?.OurPoints,
                OpponentPoints = match.MatchTracking?.OpponentPoints,
                CurrentPeriod = match.MatchTracking?.CurrentPeriod,
                StartTime = match.MatchTracking?.StartTime,
                EndTime = match.MatchTracking?.EndTime
            };

            return Task.FromResult(Result<GetMatchByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetMatchByIdResponse>.Failure($"Failed to get match: {ex.Message}"));
        }
    }
}
