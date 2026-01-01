using MediatR;
using Microsoft.AspNetCore.Http;
using TheGourmet.Application.Features.Products.Results;

namespace TheGourmet.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<CreateProductResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int StockQuantity { get; set; }
    public IFormFile? ImageFile { get; set; }
    public Guid CategoryId { get; set; }
}