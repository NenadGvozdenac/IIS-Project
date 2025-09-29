using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Teams.GetAllTeams;

public class GetAllTeamsQuery : IRequest<Result<List<GetAllTeamsResponse>>>
{
}