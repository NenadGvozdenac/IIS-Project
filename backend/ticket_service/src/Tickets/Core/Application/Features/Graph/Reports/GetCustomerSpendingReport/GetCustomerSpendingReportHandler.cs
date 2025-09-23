using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetCustomerSpendingReport;

public class GetCustomerSpendingReportHandler : IRequestHandler<GetCustomerSpendingReportQuery, Result<GetCustomerSpendingReportResponse>>
{
    private readonly IGraphReportsRepository _reportsRepository;

    public GetCustomerSpendingReportHandler(IGraphReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<Result<GetCustomerSpendingReportResponse>> Handle(GetCustomerSpendingReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customerSpending = await _reportsRepository.GetCustomerSpendingReport();

            var activeCustomers = customerSpending.Where(c => c.TotalTicketsPurchased > 0).ToList();
            var totalSpent = customerSpending.Sum(c => c.TotalAmountSpent);

            var response = new GetCustomerSpendingReportResponse
            {
                CustomerSpending = customerSpending,
                TotalCustomers = customerSpending.Count,
                TotalActiveCustomers = activeCustomers.Count,
                TotalSpentByAllCustomers = totalSpent,
                AverageSpendingPerCustomer = activeCustomers.Count > 0 ? totalSpent / activeCustomers.Count : 0
            };

            return Result<GetCustomerSpendingReportResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetCustomerSpendingReportResponse>.Failure($"An error occurred while generating customer spending report: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}