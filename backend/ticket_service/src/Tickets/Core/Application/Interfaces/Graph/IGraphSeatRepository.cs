using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphSeatRepository
{
    public Task CreateSeat(Seat seat);
    public Task<Seat?> GetSeatByName(string name);
    public Task<List<Seat>> GetAllSeats();
    public Task UpdateSeat(Seat seat);
    public Task DeleteSeat(string name);
    public Task<List<Seat>> GetSeatsByDirection(string direction);
    public Task<List<Seat>> GetSeatsByRowRange(int minRow, int maxRow);
}