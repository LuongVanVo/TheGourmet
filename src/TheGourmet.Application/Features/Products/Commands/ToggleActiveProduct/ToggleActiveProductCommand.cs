using MediatR;
using TheGourmet.Application.Features.Products.Results;

namespace TheGourmet.Application.Features.Products.Commands.ToggleActiveProduct;

public class ToggleActiveProductCommand : IRequest<ProductResponse>
{
    public Guid Id { get; set; }
}