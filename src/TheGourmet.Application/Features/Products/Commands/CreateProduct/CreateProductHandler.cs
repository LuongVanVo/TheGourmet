using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler(
    ICategoryRepository categoryRepository, 
    IProductRepository productRepository, 
    ICloudinaryService cloudinaryService, 
    IBackgroundTaskQueue backgroundTaskQueue,
    IServiceScopeFactory serviceScopeFactory)
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

        // copy file to memory BEFORE queuing background task
        byte[]? imageBytes = null;
        string? fileName = null;
        string? contentType = null;

        if (request.ImageFile != null)
        {
            using var memoryStream = new MemoryStream();
            await request.ImageFile.CopyToAsync(memoryStream, cancellationToken);
            imageBytes = memoryStream.ToArray();
            fileName = request.ImageFile.FileName;
            contentType = request.ImageFile.ContentType;
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            OriginalPrice = originalPrice,
            StockQuantity = request.StockQuantity > 0 ? request.StockQuantity : 0,
            ImageUrl = "processing...",
            CategoryId = request.CategoryId
        };

        // save to db
        var result = await productRepository.AddProductAsync(product);
        if (!result)
        {
            throw new BadRequestException("Failed to create product");
        }

        // Queue upload image if exists
        if (imageBytes != null && fileName != null && contentType != null)
        {
            var productId = product.Id;
            var capturedBytes = imageBytes;
            var capturedFileName = fileName;
            var capturedContentType = contentType;

            backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                // Create new scope for background task
                using var scope = serviceScopeFactory.CreateScope();
                var scopedProductRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                var scopedCloudinaryService = scope.ServiceProvider.GetRequiredService<ICloudinaryService>();

                // Create IFormFile from byte array
                using var stream = new MemoryStream(capturedBytes);
                var formFile = new FormFileWrapper(stream, capturedFileName, capturedContentType);

                // upload to cloudinary
                var url = await scopedCloudinaryService.UploadImageAsync(formFile, "products");

                // update product with new image URL
                var productToUpdate = await scopedProductRepository.GetProductByIdForAdminAsync(productId);
                if (productToUpdate != null)
                {
                    productToUpdate.ImageUrl = url;
                    await scopedProductRepository.UpdateProductAsync(productToUpdate);
                }
            });
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