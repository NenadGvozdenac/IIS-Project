using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.GetAllTeamMembers
{
    public class GetAllTeamMembersHandler : IRequestHandler<GetAllTeamMembersQuery, Result<GetAllTeamMembersResponse>>
    {
        private readonly ITeamMemberRepository _teamMemberRepository;

        public GetAllTeamMembersHandler(ITeamMemberRepository teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public Task<Result<GetAllTeamMembersResponse>> Handle(GetAllTeamMembersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var teamMembers = _teamMemberRepository.GetAll();

            var teamMemberResponses = teamMembers.Select(tm => new TeamMemberResponse
            {
                JerseyNumber = tm.JerseyNumber ?? 0,
                Status = tm.Status ?? string.Empty,
                IdPlayer = tm.IdPlayer,
                IdTeam = tm.IdTeam,
                PlayerName = tm.IdPlayerNavigation?.Name ?? string.Empty,
                PlayerSurname = tm.IdPlayerNavigation?.Surname ?? string.Empty,
                PlayerBirthday = tm.IdPlayerNavigation?.Birthday,
                PlayerWeight = tm.IdPlayerNavigation?.Weight,
                PlayerHeight = tm.IdPlayerNavigation?.Height,
                PlayerNationality = tm.IdPlayerNavigation?.IdNationalityNavigation?.State ?? string.Empty,
                PlayerPosition = tm.IdPlayerNavigation?.IdPositionNavigation?.Name ?? string.Empty,
                TeamName = tm.IdTeamNavigation?.Name ?? string.Empty
            });                var response = new GetAllTeamMembersResponse
                {
                    TeamMembers = teamMemberResponses
                };

                return Task.FromResult(Result<GetAllTeamMembersResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<GetAllTeamMembersResponse>.Failure($"An error occurred while retrieving team members: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError));
            }
        }
    }
}
