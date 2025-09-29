using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Competitions.GetAllCompetitions;

public class GetAllCompetitionsQuery : IRequest<Result<IEnumerable<GetAllCompetitionsResponse>>>
{
}
