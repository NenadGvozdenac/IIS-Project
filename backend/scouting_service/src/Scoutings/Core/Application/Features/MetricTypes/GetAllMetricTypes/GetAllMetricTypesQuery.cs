using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.MetricTypes.GetAllMetricTypes;

public class GetAllMetricTypesQuery : IRequest<Result<List<GetAllMetricTypesResponse>>>
{
}
