using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Matches.GetMatchesInOurHall;

public record GetMatchesInOurHallQuery() : IRequest<Result<IEnumerable<GetMatchesInOurHallResponse>>>;
