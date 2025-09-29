using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.UpdateSessionMetric;

public class UpdateSessionMetricCommand : IRequest<Result<UpdateSessionMetricResponse>>
{
    public string Value { get; set; } = null!;
    public int IdSession { get; set; }
    public int IdMetrics { get; set; }
}