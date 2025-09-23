using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Positions.CreatePosition;

public class CreatePositionCommand : IRequest<Result<CreatePositionResponse>>
{
    public string? Name { get; set; }
}
