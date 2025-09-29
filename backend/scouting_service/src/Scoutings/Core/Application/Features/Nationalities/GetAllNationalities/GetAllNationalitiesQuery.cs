using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Nationalities.GetAllNationalities;

public class GetAllNationalitiesQuery : IRequest<Result<List<GetAllNationalitiesResponse>>>
{
}
