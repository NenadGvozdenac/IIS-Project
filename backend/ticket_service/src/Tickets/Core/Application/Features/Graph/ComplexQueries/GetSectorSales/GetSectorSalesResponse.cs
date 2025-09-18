using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetSectorSales;

public class GetSectorSalesResponse
{
    public int MatchId { get; set; }
    public string MatchName { get; set; } = string.Empty;
    public List<SectorSalesDto> Sectors { get; set; } = new List<SectorSalesDto>();
}