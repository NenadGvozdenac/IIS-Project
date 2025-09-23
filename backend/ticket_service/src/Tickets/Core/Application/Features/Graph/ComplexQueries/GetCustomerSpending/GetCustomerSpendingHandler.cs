using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetCustomerSpending;

public class GetCustomerSpendingHandler : IRequestHandler<GetCustomerSpendingQuery, Result<GetCustomerSpendingResponse>>
{
    private readonly IGraphComplexQueryRepository _complexQueryRepository;

    public GetCustomerSpendingHandler(IGraphComplexQueryRepository complexQueryRepository)
    {
        _complexQueryRepository = complexQueryRepository;
    }

    public async Task<Result<GetCustomerSpendingResponse>> Handle(GetCustomerSpendingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customers = await _complexQueryRepository.GetCustomersWithHighSpending(request.MinSpending);

            var response = new GetCustomerSpendingResponse
            {
                Customers = customers
            };

            return Result<GetCustomerSpendingResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetCustomerSpendingResponse>.Failure($"An error occurred while getting customer spending: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}