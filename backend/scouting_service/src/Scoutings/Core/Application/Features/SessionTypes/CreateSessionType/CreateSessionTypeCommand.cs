using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.CreateSessionType;

public class CreateSessionTypeCommand : IRequest<Result<CreateSessionTypeResponse>>
{
    public string Name { get; set; } = string.Empty;
}
