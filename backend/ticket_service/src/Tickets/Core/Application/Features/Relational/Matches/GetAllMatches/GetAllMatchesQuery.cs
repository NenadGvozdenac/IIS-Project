using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Matches.GetAllMatches;

public record GetAllMatchesQuery() : IRequest<Result<IEnumerable<GetAllMatchesResponse>>>;
