using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.UpdateSession;

public class UpdateSessionHandler : IRequestHandler<UpdateSessionCommand, Result<UpdateSessionResponse>>
{
    private readonly ISessionRepository _sessionRepository;

    public UpdateSessionHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public Task<Result<UpdateSessionResponse>> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingSession = _sessionRepository.GetById(request.IdSession);
            if (existingSession == null)
            {
                return Task.FromResult(Result<UpdateSessionResponse>.Failure("Session not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Update the existing tracked entity instead of creating a new one
            existingSession.StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc);
            existingSession.EndTime = request.EndTime.HasValue ? DateTime.SpecifyKind(request.EndTime.Value, DateTimeKind.Utc) : null;
            existingSession.IdSessionStatus = request.IdSessionStatus;
            existingSession.IdSessionType = request.IdSessionType;
            existingSession.IdUser = request.IdUser;
            existingSession.IdPlayer = request.IdPlayer;

            var updatedSession = _sessionRepository.Update(existingSession);

            var response = new UpdateSessionResponse
            {
                IdSession = updatedSession.IdSession,
                StartTime = updatedSession.StartTime,
                EndTime = updatedSession.EndTime,
                IdSessionStatus = updatedSession.IdSessionStatus,
                IdSessionType = updatedSession.IdSessionType,
                IdUser = updatedSession.IdUser,
                IdPlayer = updatedSession.IdPlayer
            };

            return Task.FromResult(Result<UpdateSessionResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateSessionResponse>.Failure($"An error occurred while updating session: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}