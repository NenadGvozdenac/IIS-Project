namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.CheckSeasonTicketConflict;

public class CheckSeasonTicketConflictResponse
{
    public bool HasConflict { get; set; }
    public string? ConflictReason { get; set; }
    public DateTime? SeasonTicketValidFrom { get; set; }
    public string? SeasonTicketHolder { get; set; }
}
