using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.CreateMetric;

public class CreateMetricCommand : IRequest<Result<CreateMetricResponse>>
{
    public string Name { get; set; } = null!;
    public int IsPermanent { get; set; }
    public int MetricWeight { get; set; }
    public int IdUser { get; set; }
    public int IdMetricType { get; set; }
}
