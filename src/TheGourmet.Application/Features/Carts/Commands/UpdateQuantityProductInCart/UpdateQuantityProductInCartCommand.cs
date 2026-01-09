using MediatR;
using TheGourmet.Application.DTOs.Cart;

namespace TheGourmet.Application.Features.Carts.Commands.UpdateQuantityProductInCart;

public class UpdateQuantityProductInCartCommand : IRequest<CartDto>
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int NewQuantity { get; set; }
}