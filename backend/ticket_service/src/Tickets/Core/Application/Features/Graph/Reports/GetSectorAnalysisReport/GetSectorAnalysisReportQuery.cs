using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetSectorAnalysisReport;

public class GetSectorAnalysisReportQuery : IRequest<Result<GetSectorAnalysisReportResponse>>
{
    // Nema dodatnih parametara za ovaj složeni izveštaj
}