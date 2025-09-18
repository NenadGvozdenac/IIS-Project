using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetMatchAveragePrice;

public class GetMatchAveragePriceResponse
{
    public List<MatchAveragePriceDto> Matches { get; set; } = new List<MatchAveragePriceDto>();
}