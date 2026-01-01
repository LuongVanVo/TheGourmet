using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Products.Commands.ToggleActiveProduct;

public class ToggleActiveProductHandler(IProductRepository productRepository)
    : IRequestHandler<ToggleActiveProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ProductResponse> Handle(ToggleActiveProductCommand request, CancellationToken cancellationToken)
    {
        // ensure product exists
        var product = await _productRepository.GetProductByIdForAdminAsync(request.Id);
        if (product == null)
        {
            throw new NotFoundException("Product not found");
        }
        
        // toggle active status
        product.IsActive = !product.IsActive;
        
        // update product
        await _productRepository.UpdateProductAsync(product);

        return new ProductResponse
        {
            Success = true,
            Message = "Product active status toggled successfully",
        };
    }
 }