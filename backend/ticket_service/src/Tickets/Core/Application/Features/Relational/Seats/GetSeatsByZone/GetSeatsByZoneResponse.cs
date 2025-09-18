namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatsByZone;

public class GetSeatsByZoneResponse
{
    public int IdSeat { get; set; }
    public int SeatRow { get; set; }
    public int SeatNumber { get; set; }
    public string? SeatType { get; set; }
    public string? SeatDirection { get; set; }
    public string? SeatStatus { get; set; }
    public int IdZone { get; set; }
    public string? ZoneName { get; set; }
}
