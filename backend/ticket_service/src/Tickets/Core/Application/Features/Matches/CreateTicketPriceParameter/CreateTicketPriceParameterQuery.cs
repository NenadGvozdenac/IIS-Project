using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Matches.CreateTicketPriceParameter;

public record CreateTicketPriceParameterQuery(
    int MatchId,
    int ZoneId,
    int PriceFactor,
    int TimeFactor,
    int MinimumSeatPrice,
    int MaximumSeatPrice,
    int UserId
) : IRequest<Result<CreateTicketPriceParameterResponse>>;