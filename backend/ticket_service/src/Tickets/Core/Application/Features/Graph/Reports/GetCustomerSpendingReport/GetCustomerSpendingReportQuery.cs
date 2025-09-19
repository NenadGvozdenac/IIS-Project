using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetCustomerSpendingReport;

public class GetCustomerSpendingReportQuery : IRequest<Result<GetCustomerSpendingReportResponse>>
{
    // Nema dodatnih parametara za ovaj prosti izveštaj
}