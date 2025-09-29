using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.CreateSessionStatus;

public class CreateSessionStatusCommand : IRequest<Result<CreateSessionStatusResponse>>
{
    public string Name { get; set; } = string.Empty;
}
