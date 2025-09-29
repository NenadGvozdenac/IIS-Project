using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetMatchTicketSalesReport;

public class GetMatchTicketSalesReportHandler : IRequestHandler<GetMatchTicketSalesReportQuery, Result<GetMatchTicketSalesReportResponse>>
{
    private readonly IGraphReportsRepository _reportsRepository;

    public GetMatchTicketSalesReportHandler(IGraphReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<Result<GetMatchTicketSalesReportResponse>> Handle(GetMatchTicketSalesReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matchSales = await _reportsRepository.GetMatchTicketSalesReport();

            var response = new GetMatchTicketSalesReportResponse
            {
                MatchSales = matchSales,
                TotalMatches = matchSales.Count,
                TotalTicketsSold = matchSales.Sum(m => m.TotalTicketsSold),
                TotalRevenue = matchSales.Sum(m => m.TotalRevenue)
            };

            return Result<GetMatchTicketSalesReportResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetMatchTicketSalesReportResponse>.Failure($"An error occurred while generating match ticket sales report: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}