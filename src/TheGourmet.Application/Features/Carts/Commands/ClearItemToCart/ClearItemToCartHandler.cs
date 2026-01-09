using MediatR;
using TheGourmet.Application.DTOs.Cart;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Carts.Commands.ClearItemToCart;

public class ClearItemToCartHandler : IRequestHandler<ClearItemToCartCommand, CartDto>
{
    private readonly ICartRepository _cartRepository;
    public ClearItemToCartHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto> Handle(ClearItemToCartCommand request, CancellationToken cancellationToken)
    {
        // find cart to clear
        await _cartRepository.DeleteCartAsync(request.UserId);

        return new CartDto
        {
            UserId = request.UserId,
            Items = new List<CartItemDto>()
        };
    }
}