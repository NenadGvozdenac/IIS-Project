using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetMatchTicketSalesReport;

public class GetMatchTicketSalesReportQuery : IRequest<Result<GetMatchTicketSalesReportResponse>>
{
    // Nema dodatnih parametara za ovaj prosti izveštaj
}