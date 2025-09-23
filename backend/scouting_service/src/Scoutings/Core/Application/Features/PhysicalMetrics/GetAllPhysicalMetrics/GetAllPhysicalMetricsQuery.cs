using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.PhysicalMetrics.GetAllPhysicalMetrics;

public class GetAllPhysicalMetricsQuery : IRequest<Result<List<GetAllPhysicalMetricsResponse>>>
{
}
