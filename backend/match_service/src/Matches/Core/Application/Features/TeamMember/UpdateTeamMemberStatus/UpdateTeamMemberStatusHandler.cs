using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.UpdateTeamMemberStatus
{
    public class UpdateTeamMemberStatusHandler : IRequestHandler<UpdateTeamMemberStatusCommand, Result<UpdateTeamMemberStatusResponse>>
    {
        private readonly ITeamMemberRepository _teamMemberRepository;

        public UpdateTeamMemberStatusHandler(ITeamMemberRepository teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public Task<Result<UpdateTeamMemberStatusResponse>> Handle(UpdateTeamMemberStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate status value
                var validStatuses = new[] { "active", "injured", "suspended" };
                if (!validStatuses.Contains(request.Status.ToLower()))
                {
                    return Task.FromResult(Result<UpdateTeamMemberStatusResponse>.Failure($"Invalid status. Must be one of: {string.Join(", ", validStatuses)}")
                        .WithCode((int)ResultCode.BadRequest));
                }

                var existingTeamMember = _teamMemberRepository.GetById(request.IdPlayer, request.IdTeam);

                if (existingTeamMember == null)
                {
                    return Task.FromResult(Result<UpdateTeamMemberStatusResponse>.Failure($"Team member with Player ID {request.IdPlayer} and Team ID {request.IdTeam} not found.")
                        .WithCode((int)ResultCode.NotFound));
                }

                // Update only the status
                existingTeamMember.Status = request.Status.ToLower();

                _teamMemberRepository.Update(existingTeamMember);

                var response = new UpdateTeamMemberStatusResponse
                {
                    IdPlayer = request.IdPlayer,
                    IdTeam = request.IdTeam,
                    Status = existingTeamMember.Status,
                    Message = "Player status updated successfully"
                };

                return Task.FromResult(Result<UpdateTeamMemberStatusResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<UpdateTeamMemberStatusResponse>.Failure($"An error occurred while updating team member status: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError));
            }
        }
    }
}
