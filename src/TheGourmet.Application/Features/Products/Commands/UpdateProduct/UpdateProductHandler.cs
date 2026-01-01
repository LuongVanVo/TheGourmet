using MassTransit.Contracts;
using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // find product by id
        var product = await _productRepository.GetProductByIdForAdminAsync(request.Id);
        if (product == null)
        {
            throw new NotFoundException("Product not found");
        }
        
        // update product properties
        if (!string.IsNullOrWhiteSpace((request.Name)))
            product.Name = request.Name;
        
        if (!string.IsNullOrWhiteSpace((request.Description)))
            product.Description = request.Description;
        
        if (request.Price.HasValue) 
            product.Price = request.Price.Value;
         
        if (request.OriginalPrice.HasValue) 
            product.OriginalPrice = request.OriginalPrice.Value;
        
        if (request.StockQuantity.HasValue)
            product.StockQuantity = request.StockQuantity.Value;
        
        if (!string.IsNullOrWhiteSpace((request.ImageUrl)))
            product.ImageUrl = request.ImageUrl;
        
        if (request.CategoryId.HasValue)
            product.CategoryId = request.CategoryId.Value;
        
        // save changes
        await _productRepository.UpdateProductAsync(product);
        
        return new ProductResponse()
        {
            Success = true,
            Message = "Product updated successfully"
        };
    }
}