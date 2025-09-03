using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.UpdateTeamMember
{
    public class UpdateTeamMemberHandler : IRequestHandler<UpdateTeamMemberCommand, Result<UpdateTeamMemberResponse>>
    {
        private readonly ITeamMemberRepository _teamMemberRepository;

        public UpdateTeamMemberHandler(ITeamMemberRepository teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public Task<Result<UpdateTeamMemberResponse>> Handle(UpdateTeamMemberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingTeamMember = _teamMemberRepository.GetById(request.IdPlayer, request.IdTeam);

                if (existingTeamMember == null)
                {
                    return Task.FromResult(Result<UpdateTeamMemberResponse>.Failure($"Team member with Player ID {request.IdPlayer} and Team ID {request.IdTeam} not found.")
                        .WithCode((int)ResultCode.NotFound));
                }

                // Update the team member properties
                existingTeamMember.JerseyNumber = request.JerseyNumber;
                existingTeamMember.Status = request.Status;

                _teamMemberRepository.Update(existingTeamMember);

                // Get the updated team member with navigation properties
                var updatedTeamMember = _teamMemberRepository.GetById(request.IdPlayer, request.IdTeam);

                var response = new UpdateTeamMemberResponse
                {
                    TeamMember = new TeamMemberResponse
                    {
                        JerseyNumber = updatedTeamMember.JerseyNumber ?? 0,
                        Status = updatedTeamMember.Status ?? string.Empty,
                        IdPlayer = updatedTeamMember.IdPlayer,
                        IdTeam = updatedTeamMember.IdTeam,
                        PlayerName = updatedTeamMember.IdPlayerNavigation?.Name ?? string.Empty,
                        PlayerSurname = updatedTeamMember.IdPlayerNavigation?.Surname ?? string.Empty,
                        TeamName = updatedTeamMember.IdTeamNavigation?.Name ?? string.Empty
                    }
                };

                return Task.FromResult(Result<UpdateTeamMemberResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<UpdateTeamMemberResponse>.Failure($"An error occurred while updating team member: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError));
            }
        }
    }
}
