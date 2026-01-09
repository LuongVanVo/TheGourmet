using MediatR;
using TheGourmet.Application.DTOs.Cart;

namespace TheGourmet.Application.Features.Carts.Commands.RemoveItemFromCart;

public class RemoveItemFromCartCommand : IRequest<CartDto>
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
}