using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Users.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<GetUserByIdResponse>>;
