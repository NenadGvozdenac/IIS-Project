using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.CreateSessionMetric;

public class CreateSessionMetricCommand : IRequest<Result<CreateSessionMetricResponse>>
{
    public string? Value { get; set; }
    public int IdSession { get; set; }
    public int IdMetrics { get; set; }
}
