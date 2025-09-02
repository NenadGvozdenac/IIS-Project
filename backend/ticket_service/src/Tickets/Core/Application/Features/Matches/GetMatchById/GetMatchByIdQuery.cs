using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Matches.GetMatchById;

public record GetMatchByIdQuery(int Id) : IRequest<Result<GetMatchByIdResponse>>;
