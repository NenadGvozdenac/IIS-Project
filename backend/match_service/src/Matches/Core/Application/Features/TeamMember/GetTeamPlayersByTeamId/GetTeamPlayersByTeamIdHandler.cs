using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.GetTeamPlayersByTeamId;

public class GetTeamPlayersByTeamIdHandler : IRequestHandler<GetTeamPlayersByTeamIdQuery, Result<GetTeamPlayersByTeamIdResponse>>
{
    private readonly ITeamMemberRepository _teamMemberRepository;

    public GetTeamPlayersByTeamIdHandler(ITeamMemberRepository teamMemberRepository)
    {
        _teamMemberRepository = teamMemberRepository;
    }

    public Task<Result<GetTeamPlayersByTeamIdResponse>> Handle(GetTeamPlayersByTeamIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var teamMembers = _teamMemberRepository.GetByTeamId(request.TeamId);
            
            var teamPlayers = teamMembers.Select(tm => new TeamPlayerDto
            {
                PlayerId = tm.IdPlayer,
                PlayerName = tm.IdPlayerNavigation?.Name,
                PlayerSurname = tm.IdPlayerNavigation?.Surname,
                Birthday = tm.IdPlayerNavigation?.Birthday,
                Weight = tm.IdPlayerNavigation?.Weight,
                Height = tm.IdPlayerNavigation?.Height,
                PositionName = tm.IdPlayerNavigation?.IdPositionNavigation?.Name,
                NationalityName = tm.IdPlayerNavigation?.IdNationalityNavigation?.State,
                JerseyNumber = tm.JerseyNumber,
                Status = tm.Status
            }).ToList();

            var response = new GetTeamPlayersByTeamIdResponse
            {
                TeamPlayers = teamPlayers
            };

            return Task.FromResult(Result<GetTeamPlayersByTeamIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetTeamPlayersByTeamIdResponse>.Failure($"Failed to get team players: {ex.Message}"));
        }
    }
}
