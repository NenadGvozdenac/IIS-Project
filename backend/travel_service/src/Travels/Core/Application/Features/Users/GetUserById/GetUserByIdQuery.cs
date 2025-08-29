using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Users.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<GetUserByIdResponse>>;
