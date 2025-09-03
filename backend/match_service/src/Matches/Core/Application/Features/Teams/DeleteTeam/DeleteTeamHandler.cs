using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.Teams.DeleteTeam;

public class DeleteTeamHandler : IRequestHandler<DeleteTeamCommand, Result<DeleteTeamResponse>>
{
    private readonly ITeamRepository _teamRepository;

    public DeleteTeamHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public Task<Result<DeleteTeamResponse>> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Proveri da li tim postoji
            if (!_teamRepository.Exists(request.Id))
            {
                return Task.FromResult(Result<DeleteTeamResponse>.Failure($"Team with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var deleted = _teamRepository.Delete(request.Id);

            if (!deleted)
            {
                return Task.FromResult(Result<DeleteTeamResponse>.Failure($"Failed to delete team with ID {request.Id}")
                    .WithCode((int)ResultCode.InternalServerError));
            }

            var response = new DeleteTeamResponse
            {
                Success = true,
                Message = $"Team with ID {request.Id} has been successfully deleted"
            };

            return Task.FromResult(Result<DeleteTeamResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DeleteTeamResponse>.Failure($"An error occurred while deleting the team: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
