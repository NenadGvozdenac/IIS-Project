using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.DeleteTeamMember
{
    public class DeleteTeamMemberHandler : IRequestHandler<DeleteTeamMemberCommand, Result<DeleteTeamMemberResponse>>
    {
        private readonly ITeamMemberRepository _teamMemberRepository;

        public DeleteTeamMemberHandler(ITeamMemberRepository teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public Task<Result<DeleteTeamMemberResponse>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!_teamMemberRepository.Exists(request.IdPlayer, request.IdTeam))
                {
                    return Task.FromResult(Result<DeleteTeamMemberResponse>.Failure($"Team member with Player ID {request.IdPlayer} and Team ID {request.IdTeam} not found.")
                        .WithCode((int)ResultCode.NotFound));
                }

                var deleted = _teamMemberRepository.Delete(request.IdPlayer, request.IdTeam);

                if (!deleted)
                {
                    return Task.FromResult(Result<DeleteTeamMemberResponse>.Failure("Failed to delete team member.")
                        .WithCode((int)ResultCode.InternalServerError));
                }

                var response = new DeleteTeamMemberResponse
                {
                    IsDeleted = true,
                    Message = $"Team member with Player ID {request.IdPlayer} and Team ID {request.IdTeam} has been successfully deleted."
                };

                return Task.FromResult(Result<DeleteTeamMemberResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<DeleteTeamMemberResponse>.Failure($"An error occurred while deleting team member: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError));
            }
        }
    }
}
