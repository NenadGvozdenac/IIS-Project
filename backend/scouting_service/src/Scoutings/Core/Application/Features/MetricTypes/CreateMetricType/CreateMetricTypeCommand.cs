using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.MetricTypes.CreateMetricType;

public class CreateMetricTypeCommand : IRequest<Result<CreateMetricTypeResponse>>
{
    public string? Type { get; set; }
}
