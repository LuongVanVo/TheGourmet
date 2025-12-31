using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler(ICategoryRepository categoryRepository, IProductRepository productRepository)
    : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // ensure category exists
        var category = await categoryRepository.GetCategoryByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new BadRequestException($"Not found category with id {request.CategoryId}");
        }
        
        var originalPrice = !request.OriginalPrice.HasValue || request.OriginalPrice <= 0 || request.OriginalPrice > request.Price
            ? request.Price
            : request.OriginalPrice;
        
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            OriginalPrice = originalPrice,
            StockQuantity = request.StockQuantity > 0 ? request.StockQuantity : 0,
            ImageUrl = request.ImageUrl ?? string.Empty,
            CategoryId = request.CategoryId
        };
        
        // save to db
        var result = await productRepository.AddProductAsync(product);
        if (!result)
        {
            throw new BadRequestException("Failed to create product");
        }

        return new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            OriginalPrice = product.OriginalPrice ?? 0,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl ?? string.Empty,
            CategoryId = product.CategoryId,
        };
    }
}