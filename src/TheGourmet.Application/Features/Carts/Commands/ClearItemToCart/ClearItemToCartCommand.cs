using MediatR;
using TheGourmet.Application.DTOs.Cart;

namespace TheGourmet.Application.Features.Carts.Commands.ClearItemToCart;

public class ClearItemToCartCommand : IRequest<CartDto>
{
    public Guid UserId { get; set; }
}