using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.GetAllMetrics;

public class GetAllMetricsQuery : IRequest<Result<List<GetAllMetricsResponse>>>
{
}
