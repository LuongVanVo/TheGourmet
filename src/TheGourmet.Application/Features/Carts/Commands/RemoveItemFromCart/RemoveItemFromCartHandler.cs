using MediatR;
using TheGourmet.Application.DTOs.Cart;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Carts.Commands.RemoveItemFromCart;

public class RemoveItemFromCartHandler : IRequestHandler<RemoveItemFromCartCommand, CartDto>
{
    private readonly ICartRepository _cartRepository;
    public RemoveItemFromCartHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        // find cart of user 
        var cart = await _cartRepository.GetCartAsync(request.UserId);

        // find item to remove
        var itemToRemove = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

        if (itemToRemove == null)
        {
            throw new NotFoundException("Product item not found in cart");
        }
        
        // remove item from cart
        cart.Items.Remove(itemToRemove);
        
        // update cart in repository
        await _cartRepository.UpdateCartAsync(request.UserId, cart);

        return cart;
    }
}