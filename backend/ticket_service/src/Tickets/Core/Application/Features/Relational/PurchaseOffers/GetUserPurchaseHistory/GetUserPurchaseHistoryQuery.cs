using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.GetUserPurchaseHistory;

public record GetUserPurchaseHistoryQuery(int UserId) : IRequest<Result<IEnumerable<GetUserPurchaseHistoryResponse>>>;
