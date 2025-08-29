using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Users.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<GetUserByIdResponse>>;
