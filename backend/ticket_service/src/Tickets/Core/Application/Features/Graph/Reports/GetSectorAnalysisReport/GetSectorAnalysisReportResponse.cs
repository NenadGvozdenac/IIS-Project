using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetSectorAnalysisReport;

public class GetSectorAnalysisReportResponse
{
    public SectorAnalysisReportDto SectorAnalysis { get; set; } = new SectorAnalysisReportDto();
}