using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.UpdateSessionStatus;

public class UpdateSessionStatusHandler : IRequestHandler<UpdateSessionStatusCommand, Result<UpdateSessionStatusResponse>>
{
    private readonly ISessionStatusRepository _sessionStatusRepository;

    public UpdateSessionStatusHandler(ISessionStatusRepository sessionStatusRepository)
    {
        _sessionStatusRepository = sessionStatusRepository;
    }

    public Task<Result<UpdateSessionStatusResponse>> Handle(UpdateSessionStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingSessionStatus = _sessionStatusRepository.GetById(request.IdStatus);
            if (existingSessionStatus == null)
            {
                return Task.FromResult(Result<UpdateSessionStatusResponse>.Failure("SessionStatus not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var sessionStatus = new SessionStatus
            {
                IdStatus = request.IdStatus,
                Status = request.Status
            };

            var updatedSessionStatus = _sessionStatusRepository.Update(sessionStatus);

            var response = new UpdateSessionStatusResponse
            {
                IdStatus = updatedSessionStatus.IdStatus,
                Status = updatedSessionStatus.Status
            };

            return Task.FromResult(Result<UpdateSessionStatusResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateSessionStatusResponse>.Failure($"An error occurred while updating session status: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}