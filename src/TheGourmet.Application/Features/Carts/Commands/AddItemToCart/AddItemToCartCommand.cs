using MediatR;
using TheGourmet.Application.DTOs.Cart;

namespace TheGourmet.Application.Features.Carts.Commands.AddItemToCart;

public class AddItemToCartCommand : IRequest<CartDto>
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}