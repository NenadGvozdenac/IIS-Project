using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphSeatRepository
{
    public Task<Seat?> CreateSeat(Seat seat);
    public Task<Seat?> GetSeatByName(string name);
    public Task<Seat?> GetSeatById(int id);
    public Task<List<Seat>> GetAllSeats();
    public Task<Seat?> UpdateSeat(int id, Seat seat);
    public Task DeleteSeat(int id);
}