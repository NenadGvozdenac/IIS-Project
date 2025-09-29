using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.CreditCards.GetAllCreditCards;

public class GetAllCreditCardsQuery : IRequest<Result<List<GetAllCreditCardsResponse>>>
{
}
