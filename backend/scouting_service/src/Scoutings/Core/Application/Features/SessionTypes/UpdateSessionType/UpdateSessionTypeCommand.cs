using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.UpdateSessionType;

public class UpdateSessionTypeCommand : IRequest<Result<UpdateSessionTypeResponse>>
{
    public int IdType { get; set; }
    public string Type { get; set; } = null!;
}