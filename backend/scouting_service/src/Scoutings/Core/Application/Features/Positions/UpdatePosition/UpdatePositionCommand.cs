using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Positions.UpdatePosition;

public class UpdatePositionCommand : IRequest<Result<UpdatePositionResponse>>
{
    public int IdPosition { get; set; }
    public string Name { get; set; } = null!;
}