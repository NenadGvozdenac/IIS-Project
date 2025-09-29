using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Teams.GetTeamById;

public record GetTeamByIdQuery(int Id) : IRequest<Result<GetTeamByIdResponse>>;
