using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Relational;

public class TicketPriceParameterRepository : ITicketPriceParameterRepository
{
    private readonly TicketDbContext _context;

    public TicketPriceParameterRepository(TicketDbContext context)
    {
        _context = context;
    }

    public TicketPriceParameter? GetByMatchZoneAndUser(int matchId, int zoneId, int? userId = null)
    {
        var query = _context.TicketPriceParameters
            .Include(tpp => tpp.IdMatchNavigation)
            .Include(tpp => tpp.IdZoneNavigation)
            .Include(tpp => tpp.IdUserNavigation)
            .Where(tpp => tpp.IdMatch == matchId && tpp.IdZone == zoneId);

        if (userId.HasValue)
        {
            query = query.Where(tpp => tpp.IdUser == userId.Value);
        }

        return query.FirstOrDefault();
    }

    public IEnumerable<TicketPriceParameter> GetByMatch(int matchId)
    {
        return _context.TicketPriceParameters
            .Include(tpp => tpp.IdMatchNavigation)
            .Include(tpp => tpp.IdZoneNavigation)
            .Include(tpp => tpp.IdUserNavigation)
            .Where(tpp => tpp.IdMatch == matchId)
            .ToList();
    }

    public IEnumerable<TicketPriceParameter> GetByZone(int zoneId)
    {
        return _context.TicketPriceParameters
            .Include(tpp => tpp.IdMatchNavigation)
            .Include(tpp => tpp.IdZoneNavigation)
            .Include(tpp => tpp.IdUserNavigation)
            .Where(tpp => tpp.IdZone == zoneId)
            .ToList();
    }

    public TicketPriceParameter? GetById(int id)
    {
        return _context.TicketPriceParameters
            .Include(tpp => tpp.IdMatchNavigation)
            .Include(tpp => tpp.IdZoneNavigation)
            .Include(tpp => tpp.IdUserNavigation)
            .FirstOrDefault(tpp => tpp.IdTicketPriceParameter == id);
    }

    public TicketPriceParameter Create(TicketPriceParameter parameter)
    {
        _context.TicketPriceParameters.Add(parameter);
        _context.SaveChanges();
        return parameter;
    }

    public TicketPriceParameter Update(TicketPriceParameter parameter)
    {
        _context.TicketPriceParameters.Update(parameter);
        _context.SaveChanges();
        return parameter;
    }

    public bool Delete(int id)
    {
        var parameter = _context.TicketPriceParameters.Find(id);
        if (parameter == null) return false;

        _context.TicketPriceParameters.Remove(parameter);
        _context.SaveChanges();
        return true;
    }
}
