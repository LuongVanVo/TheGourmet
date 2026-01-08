using MediatR;
using TheGourmet.Application.DTOs.Cart;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Carts.Queries.GetCart;

public class GetCartHandler : IRequestHandler<GetCartQuery, CartDto> 
{
    private readonly ICartRepository _cartRepository;
    public GetCartHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        return await _cartRepository.GetCartAsync(request.UserId);
    }
}