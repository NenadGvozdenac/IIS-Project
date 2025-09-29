using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.CreateSession;

public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, Result<CreateSessionResponse>>
{
    private readonly ISessionRepository _sessionRepository;

    public CreateSessionHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public Task<Result<CreateSessionResponse>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var session = new Session
        {
            StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc),
            EndTime = request.EndTime.HasValue ? DateTime.SpecifyKind(request.EndTime.Value, DateTimeKind.Utc) : null,
            IdSessionStatus = request.IdSessionStatus,
            IdSessionType = request.IdSessionType,
            IdUser = request.IdUser,
            IdPlayer = request.IdPlayer
        };

        var createdSession = _sessionRepository.Create(session);

        if (createdSession == null)
        {
            return Task.FromResult(Result<CreateSessionResponse>.Failure("Failed to create session"));
        }

        var response = new CreateSessionResponse
        {
            Id = createdSession.IdSession,
            StartTime = createdSession.StartTime,
            EndTime = createdSession.EndTime,
            IdSessionStatus = createdSession.IdSessionStatus,
            IdSessionType = createdSession.IdSessionType,
            IdUser = createdSession.IdUser,
            IdPlayer = createdSession.IdPlayer
        };

        return Task.FromResult(Result<CreateSessionResponse>.Success(response));
    }
}
