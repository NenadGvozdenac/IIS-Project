using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.GetAllSessionMetrics;

public class GetAllSessionMetricsQuery : IRequest<Result<GetAllSessionMetricsResponse>>
{
}
