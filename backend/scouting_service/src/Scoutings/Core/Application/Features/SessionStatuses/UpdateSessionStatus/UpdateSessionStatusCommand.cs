using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.UpdateSessionStatus;

public class UpdateSessionStatusCommand : IRequest<Result<UpdateSessionStatusResponse>>
{
    public int IdStatus { get; set; }
    public string Status { get; set; } = null!;
}