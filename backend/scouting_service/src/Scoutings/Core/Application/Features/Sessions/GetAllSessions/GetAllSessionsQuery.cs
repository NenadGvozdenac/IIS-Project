using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.GetAllSessions;

public class GetAllSessionsQuery : IRequest<Result<List<GetAllSessionsResponse>>>
{
}
