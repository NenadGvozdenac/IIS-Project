using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;
using ticket_service.src.Tickets.Core.Infrastructure;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public CartRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public Cart? GetById(int id)
    {
        return _ticketDbContext.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.IdPurchaseOfferNavigation)
            .Include(c => c.IdUserNavigation)
            .Include(c => c.IdCreditCardNavigation)
            .FirstOrDefault(c => c.IdCart == id);
    }

    public IEnumerable<Cart> GetAll()
    {
        return _ticketDbContext.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.IdPurchaseOfferNavigation)
            .Include(c => c.IdUserNavigation)
            .Include(c => c.IdCreditCardNavigation)
            .ToList();
    }

    public IEnumerable<Cart> GetByUserId(int userId)
    {
        return _ticketDbContext.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.IdPurchaseOfferNavigation)
            .Include(c => c.IdUserNavigation)
            .Include(c => c.IdCreditCardNavigation)
            .Where(c => c.IdUser == userId)
            .ToList();
    }

    public Cart? GetCurrentCartByUserId(int userId)
    {
        return _ticketDbContext.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.IdPurchaseOfferNavigation)
            .Include(c => c.IdUserNavigation)
            .Include(c => c.IdCreditCardNavigation)
            .FirstOrDefault(c => c.IdUser == userId && c.IsCurrent);
    }

    public Cart Create(Cart cart)
    {
        _ticketDbContext.Carts.Add(cart);
        _ticketDbContext.SaveChanges();
        return cart;
    }

    public Cart Update(Cart cart)
    {
        _ticketDbContext.Carts.Update(cart);
        _ticketDbContext.SaveChanges();
        return cart;
    }

    public bool Delete(int id)
    {
        var cart = _ticketDbContext.Carts.Find(id);
        if (cart == null) return false;

        _ticketDbContext.Carts.Remove(cart);
        _ticketDbContext.SaveChanges();
        return true;
    }
}
