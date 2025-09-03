using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Seasons.GetAllSeasons;

public class GetAllSeasonsQuery : IRequest<Result<List<GetAllSeasonsResponse>>>
{
}
