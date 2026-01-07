using AutoMapper;
using MassTransit.Contracts;
using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // find product by id
        var product = await _productRepository.GetProductByIdForAdminAsync(request.Id);
        if (product == null)
        {
            throw new NotFoundException("Product not found");
        }
        
        // map updated fields
        mapper.Map(request, product);
        
        // set audit fields
        product.LastModified = DateTime.UtcNow;
        
        // save changes
        await _productRepository.UpdateProductAsync(product);
        
        return new ProductResponse()
        {
            Success = true,
            Message = "Product updated successfully"
        };
    }
}