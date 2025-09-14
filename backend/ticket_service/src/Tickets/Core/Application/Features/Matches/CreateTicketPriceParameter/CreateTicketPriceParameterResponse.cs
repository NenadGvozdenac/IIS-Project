namespace ticket_service.src.Tickets.Core.Application.Features.Matches.CreateTicketPriceParameter;

public class CreateTicketPriceParameterResponse
{
    public int IdTicketPriceParameter { get; set; }
    public int MatchId { get; set; }
    public string MatchName { get; set; } = null!;
    public int ZoneId { get; set; }
    public string ZoneName { get; set; } = null!;
    public int PriceFactor { get; set; }
    public int TimeFactor { get; set; }
    public int MinimumSeatPrice { get; set; }
    public int MaximumSeatPrice { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; } = null!;
}