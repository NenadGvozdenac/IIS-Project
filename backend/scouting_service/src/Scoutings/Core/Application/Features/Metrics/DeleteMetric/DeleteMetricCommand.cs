using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.DeleteMetric;

public class DeleteMetricCommand : IRequest<Result<DeleteMetricResponse>>
{
    public int IdMetrics { get; set; }
}
