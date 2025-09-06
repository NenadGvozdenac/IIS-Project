using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.GetTeamMembersByMatch;

public class GetTeamMembersByMatchHandler : IRequestHandler<GetTeamMembersByMatchQuery, Result<GetTeamMembersByMatchResponse>>
{
    private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;

    public GetTeamMembersByMatchHandler(ITeamMemberMatchRepository teamMemberMatchRepository)
    {
        _teamMemberMatchRepository = teamMemberMatchRepository;
    }

    public Task<Result<GetTeamMembersByMatchResponse>> Handle(GetTeamMembersByMatchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var teamMembers = request.TeamId.HasValue
                ? _teamMemberMatchRepository.GetByMatchAndTeamId(request.MatchId, request.TeamId.Value)
                : _teamMemberMatchRepository.GetByMatchId(request.MatchId);
            
            var teamMemberDtos = teamMembers.Select(tm => new TeamMemberDto
            {
                IdMatch = tm.IdMatch,
                IdTeam = tm.IdTeam,
                IdPlayer = tm.IdPlayer,
                PlayerName = tm.Id?.IdPlayerNavigation?.Name ?? "",
                PlayerSurname = tm.Id?.IdPlayerNavigation?.Surname ?? "",
                JerseyNumber = tm.Id?.JerseyNumber,
                PositionName = tm.Id?.IdPlayerNavigation?.IdPositionNavigation?.Name,
                InGame = tm.InGame,
                StartingLineup = tm.StartingLineup,
                Age = tm.Id?.IdPlayerNavigation?.Birthday != null 
                    ? DateTime.Now.Year - tm.Id.IdPlayerNavigation.Birthday.Value.Year
                    : null,
                TeamName = tm.Id?.IdTeamNavigation?.Name ?? ""
            }).ToList();

            var response = new GetTeamMembersByMatchResponse
            {
                TeamMembers = teamMemberDtos
            };

            return Task.FromResult(Result<GetTeamMembersByMatchResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetTeamMembersByMatchResponse>.Failure($"Failed to get team members: {ex.Message}"));
        }
    }
}
