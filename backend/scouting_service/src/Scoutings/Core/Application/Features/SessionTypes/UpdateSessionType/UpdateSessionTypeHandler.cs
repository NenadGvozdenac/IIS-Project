using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.UpdateSessionType;

public class UpdateSessionTypeHandler : IRequestHandler<UpdateSessionTypeCommand, Result<UpdateSessionTypeResponse>>
{
    private readonly ISessionTypeRepository _sessionTypeRepository;

    public UpdateSessionTypeHandler(ISessionTypeRepository sessionTypeRepository)
    {
        _sessionTypeRepository = sessionTypeRepository;
    }

    public Task<Result<UpdateSessionTypeResponse>> Handle(UpdateSessionTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingSessionType = _sessionTypeRepository.GetById(request.IdType);
            if (existingSessionType == null)
            {
                return Task.FromResult(Result<UpdateSessionTypeResponse>.Failure("SessionType not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var sessionType = new SessionType
            {
                IdType = request.IdType,
                Type = request.Type
            };

            var updatedSessionType = _sessionTypeRepository.Update(sessionType);

            var response = new UpdateSessionTypeResponse
            {
                IdType = updatedSessionType.IdType,
                Type = updatedSessionType.Type
            };

            return Task.FromResult(Result<UpdateSessionTypeResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateSessionTypeResponse>.Failure($"An error occurred while updating session type: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}