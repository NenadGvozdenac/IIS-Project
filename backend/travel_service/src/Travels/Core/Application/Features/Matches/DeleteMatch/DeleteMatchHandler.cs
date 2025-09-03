using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Matches.DeleteMatch;

public class DeleteMatchHandler : IRequestHandler<DeleteMatchCommand, Result<DeleteMatchResponse>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;

    public DeleteMatchHandler(IMatchRepository matchRepository, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
    }

    public Task<Result<DeleteMatchResponse>> Handle(DeleteMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<DeleteMatchResponse>.Failure("User not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            if (user.Type != "team manager")
            {
                return Task.FromResult(Result<DeleteMatchResponse>.Failure("Only team manager can delete matches")
                    .WithCode((int)ResultCode.Forbidden));
            }

            var match = _matchRepository.GetById(request.Id);
            if (match == null)
            {
                return Task.FromResult(Result<DeleteMatchResponse>.Failure($"Match with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            if (match.ScheduledAt <= DateTime.Now)
            {
                return Task.FromResult(Result<DeleteMatchResponse>.Failure("Cannot delete match that has already started or finished")
                    .WithCode((int)ResultCode.Forbidden));
            }

            var isDeleted = _matchRepository.Delete(request.Id);

            if (!isDeleted)
            {
                return Task.FromResult(Result<DeleteMatchResponse>.Failure($"Failed to delete match with ID {request.Id}")
                    .WithCode((int)ResultCode.InternalServerError));
            }

            var response = new DeleteMatchResponse
            {
                Success = true,
                Message = $"Match with ID {request.Id} has been successfully deleted"
            };

            return Task.FromResult(Result<DeleteMatchResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DeleteMatchResponse>.Failure($"An error occurred while deleting the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}