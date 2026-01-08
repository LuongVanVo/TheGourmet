using TheGourmet.Application.DTOs.Cart;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface ICartRepository
{
    // Get cart by user id
    Task<CartDto?> GetCartAsync(Guid userId);
    
    // Update cart 
    Task UpdateCartAsync(Guid userId, CartDto cartDto);
    
    // Delete cart
    Task DeleteCartAsync(Guid userId);
}