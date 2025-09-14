using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Matches.EnableTicketsForUpcomingMatches;

public record EnableTicketsForUpcomingMatchesQuery(int MatchId) : IRequest<Result<EnableTicketsForUpcomingMatchesResponse>>;