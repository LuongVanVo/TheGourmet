using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdHandler> _logger;
    public GetProductByIdHandler(IProductRepository productRepository, IMapper mapper, ILogger<GetProductByIdHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(request.Id);
        if (product == null)
        {
            _logger.LogWarning("Product with id {ProductId} not found", request.Id);
            throw new NotFoundException(nameof(Product), request.Id);
        }

        if (!product.IsActive)
        {
            _logger.LogWarning("Product with id {ProductId} is inactive", request.Id);
            throw new NotFoundException(nameof(Product), request.Id);
        }
        
        var response = _mapper.Map<GetProductByIdResponse>(product);
        return response;    
    }
}