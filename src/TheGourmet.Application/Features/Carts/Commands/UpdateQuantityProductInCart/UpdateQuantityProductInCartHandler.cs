using MediatR;
using TheGourmet.Application.DTOs.Cart;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Carts.Commands.UpdateQuantityProductInCart;

public class UpdateQuantityProductInCartHandler : IRequestHandler<UpdateQuantityProductInCartCommand, CartDto>
{
    private readonly ICartRepository _cartRepository;
    public UpdateQuantityProductInCartHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto> Handle(UpdateQuantityProductInCartCommand request, CancellationToken cancellationToken)
    {
        // find cart 
        var cart = await _cartRepository.GetCartAsync(request.UserId);
        
        // find product in cart
        var itemProduct = cart?.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

        if (itemProduct == null)
        {
            throw new NotFoundException("Product not found in cart");
        }

        if (request.NewQuantity <= 0)
        {
            // if new quantity is less than or equal to 0, remove item from cart
            cart.Items.Remove(itemProduct);
            await _cartRepository.UpdateCartAsync(request.UserId, cart);
        }
        else
        {
            // Update new quantity
            itemProduct.Quantity = request.NewQuantity;
            
            // check stock remaining
            if (itemProduct.Quantity > itemProduct.StockRemaining) 
            {
                throw new BadRequestException("Insufficient stock for the requested quantity.");
            }
            await _cartRepository.UpdateCartAsync(request.UserId, cart);
        }

        return cart;
    }
}