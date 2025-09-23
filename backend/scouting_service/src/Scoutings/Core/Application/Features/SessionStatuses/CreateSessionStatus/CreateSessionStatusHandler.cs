using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.CreateSessionStatus;

public class CreateSessionStatusHandler : IRequestHandler<CreateSessionStatusCommand, Result<CreateSessionStatusResponse>>
{
    private readonly ISessionStatusRepository _sessionStatusRepository;

    public CreateSessionStatusHandler(ISessionStatusRepository sessionStatusRepository)
    {
        _sessionStatusRepository = sessionStatusRepository;
    }

    public Task<Result<CreateSessionStatusResponse>> Handle(CreateSessionStatusCommand request, CancellationToken cancellationToken)
    {
        var sessionStatus = new SessionStatus
        {
            Status = request.Name
        };

        var createdSessionStatus = _sessionStatusRepository.Create(sessionStatus);

        if (createdSessionStatus == null)
        {
            return Task.FromResult(Result<CreateSessionStatusResponse>.Failure("Failed to create session status"));
        }

        var response = new CreateSessionStatusResponse
        {
            Id = createdSessionStatus.IdStatus,
            Name = createdSessionStatus.Status ?? string.Empty
        };

        return Task.FromResult(Result<CreateSessionStatusResponse>.Success(response));
    }
}
