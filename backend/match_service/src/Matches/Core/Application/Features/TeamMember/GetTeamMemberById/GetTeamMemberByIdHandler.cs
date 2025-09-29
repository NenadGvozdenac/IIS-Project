using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.GetTeamMemberById
{
    public class GetTeamMemberByIdHandler : IRequestHandler<GetTeamMemberByIdQuery, Result<GetTeamMemberByIdResponse>>
    {
        private readonly ITeamMemberRepository _teamMemberRepository;

        public GetTeamMemberByIdHandler(ITeamMemberRepository teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public Task<Result<GetTeamMemberByIdResponse>> Handle(GetTeamMemberByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var teamMember = _teamMemberRepository.GetById(request.IdPlayer, request.IdTeam);

                if (teamMember == null)
                {
                    return Task.FromResult(Result<GetTeamMemberByIdResponse>.Failure($"Team member with Player ID {request.IdPlayer} and Team ID {request.IdTeam} not found.")
                        .WithCode((int)ResultCode.NotFound));
                }

                var response = new GetTeamMemberByIdResponse
                {
                    TeamMember = new TeamMemberResponse
                    {
                        JerseyNumber = teamMember.JerseyNumber ?? 0,
                        Status = teamMember.Status ?? string.Empty,
                        IdPlayer = teamMember.IdPlayer,
                        IdTeam = teamMember.IdTeam,
                        PlayerName = teamMember.IdPlayerNavigation?.Name ?? string.Empty,
                        PlayerSurname = teamMember.IdPlayerNavigation?.Surname ?? string.Empty,
                        PlayerBirthday = teamMember.IdPlayerNavigation?.Birthday,
                        PlayerWeight = teamMember.IdPlayerNavigation?.Weight,
                        PlayerHeight = teamMember.IdPlayerNavigation?.Height,
                        PlayerNationality = teamMember.IdPlayerNavigation?.IdNationalityNavigation?.State ?? string.Empty,
                        PlayerPosition = teamMember.IdPlayerNavigation?.IdPositionNavigation?.Name ?? string.Empty,
                        TeamName = teamMember.IdTeamNavigation?.Name ?? string.Empty
                    }
                };

                return Task.FromResult(Result<GetTeamMemberByIdResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<GetTeamMemberByIdResponse>.Failure($"An error occurred while retrieving team member: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError));
            }
        }
    }
}
