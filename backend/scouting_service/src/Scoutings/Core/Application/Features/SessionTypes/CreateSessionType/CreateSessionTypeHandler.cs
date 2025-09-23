using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.CreateSessionType;

public class CreateSessionTypeHandler : IRequestHandler<CreateSessionTypeCommand, Result<CreateSessionTypeResponse>>
{
    private readonly ISessionTypeRepository _sessionTypeRepository;

    public CreateSessionTypeHandler(ISessionTypeRepository sessionTypeRepository)
    {
        _sessionTypeRepository = sessionTypeRepository;
    }

    public Task<Result<CreateSessionTypeResponse>> Handle(CreateSessionTypeCommand request, CancellationToken cancellationToken)
    {
        var sessionType = new SessionType
        {
            Type = request.Name
        };

        var createdSessionType = _sessionTypeRepository.Create(sessionType);

        if (createdSessionType == null)
        {
            return Task.FromResult(Result<CreateSessionTypeResponse>.Failure("Failed to create session type"));
        }

        var response = new CreateSessionTypeResponse
        {
            Id = createdSessionType.IdType,
            Name = createdSessionType.Type ?? string.Empty
        };

        return Task.FromResult(Result<CreateSessionTypeResponse>.Success(response));
    }
}
