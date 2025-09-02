using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface ICartRepository
{
    Cart? GetById(int id);
    IEnumerable<Cart> GetAll();
    IEnumerable<Cart> GetByUserId(int userId);
    Cart? GetCurrentCartByUserId(int userId);
    Cart Create(Cart cart);
    Cart Update(Cart cart);
    bool Delete(int id);
    CartItem AddItemToCart(CartItem cartItem);
    bool RemoveItemFromCart(int cartId, int purchaseOfferId);
}
