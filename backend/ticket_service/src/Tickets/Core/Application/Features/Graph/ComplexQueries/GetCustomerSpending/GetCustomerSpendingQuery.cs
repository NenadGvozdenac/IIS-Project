using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetCustomerSpending;

public class GetCustomerSpendingQuery : IRequest<Result<GetCustomerSpendingResponse>>
{
    public decimal MinSpending { get; set; }

    public GetCustomerSpendingQuery(decimal minSpending = 4000)
    {
        MinSpending = minSpending;
    }
}