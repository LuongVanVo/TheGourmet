using MediatR;
using TheGourmet.Application.DTOs.Cart;

namespace TheGourmet.Application.Features.Carts.Queries.GetCart;

public class GetCartQuery : IRequest<CartDto>
{
    public Guid UserId { get; set; }

    public GetCartQuery(Guid userId)
    {
        UserId = userId;
    }
}