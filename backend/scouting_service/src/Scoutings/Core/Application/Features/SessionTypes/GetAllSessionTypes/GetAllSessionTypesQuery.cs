using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.GetAllSessionTypes;

public class GetAllSessionTypesQuery : IRequest<Result<GetAllSessionTypesResponse>>
{
}
