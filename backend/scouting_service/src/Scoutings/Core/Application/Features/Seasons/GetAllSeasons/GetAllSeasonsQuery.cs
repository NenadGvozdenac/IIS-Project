using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Seasons.GetAllSeasons;

public class GetAllSeasonsQuery : IRequest<Result<GetAllSeasonsResponse>>
{
}
