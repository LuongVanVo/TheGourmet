using MediatR;
using TheGourmet.Application.Features.Products.Results;

namespace TheGourmet.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<GetProductByIdResponse>
{
    public Guid Id { get; set; }
}