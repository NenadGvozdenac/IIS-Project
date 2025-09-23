using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.GetAllSessionStatuses;

public class GetAllSessionStatusesQuery : IRequest<Result<GetAllSessionStatusesResponse>>
{
}
