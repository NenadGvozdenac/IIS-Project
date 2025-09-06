using ticket_service.src.Tickets.Core.Infrastructure;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public SeatRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public Seat? GetById(int id)
    {
        return _ticketDbContext.Seats
            .Include(s => s.IdZoneNavigation)
            .FirstOrDefault(s => s.IdSeat == id);
    }

    public IEnumerable<Seat> GetAll()
    {
        return _ticketDbContext.Seats
            .Include(s => s.IdZoneNavigation)
            .ToList();
    }

    public IEnumerable<Seat> GetByZoneId(int zoneId)
    {
        return _ticketDbContext.Seats
            .Include(s => s.IdZoneNavigation)
            .Where(s => s.IdZone == zoneId)
            .ToList();
    }

    public Seat? GetByZoneAndPosition(int zoneId, int row, int number)
    {
        return _ticketDbContext.Seats
            .Include(s => s.IdZoneNavigation)
            .FirstOrDefault(s => s.IdZone == zoneId && s.Row == row && s.Number == number);
    }

    public Seat? GetByZoneAndPositionAndDirection(int zoneId, int row, int number, string direction)
    {
        return _ticketDbContext.Seats
            .Include(s => s.IdZoneNavigation)
            .FirstOrDefault(s => s.IdZone == zoneId && s.Row == row && s.Number == number && s.Direction.ToLower() == direction.ToLower());
    }

    public Seat Create(Seat seat)
    {
        _ticketDbContext.Seats.Add(seat);
        _ticketDbContext.SaveChanges();
        return seat;
    }

    public Seat Update(Seat seat)
    {
        _ticketDbContext.Seats.Update(seat);
        _ticketDbContext.SaveChanges();
        return seat;
    }

    public bool Delete(int id)
    {
        var seat = _ticketDbContext.Seats.Find(id);
        if (seat == null) return false;

        _ticketDbContext.Seats.Remove(seat);
        _ticketDbContext.SaveChanges();
        return true;
    }
}
