using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.ProductReviews.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.ProductReviews.Commands.CreateReview;

public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, ProductReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateReviewHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductReviewResponse> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null || order.UserId != request.UserId)
        {
            throw new BadRequestException("Invalid order for review.");
        }

        if (order.Status != OrderStatus.Completed)
        {
            throw new BadRequestException("You can only review products from completed orders.");
        }

        var hasProduct = order.OrderItems.Any(x => x.ProductId == request.ProductId);
        if (!hasProduct)
        {
            throw new BadRequestException("The product is not part of the specified order.");
        }

        var existingReview =
            await _unitOfWork.ProductReviews.HasUserReviewedOrderedProductAsync(request.UserId,
                request.ProductId, request.OrderId);
        if (existingReview)
            throw new BadRequestException("You have already reviewed this product for the specified order.");
        
        // create review
        var review = new ProductReview()
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            OrderId = request.OrderId,
            Rating = request.Rating,
            Comments = request.Comments,
            CreatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.ProductReviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var allReviews = await _unitOfWork.ProductReviews.GetByProductIdAsync(request.ProductId);

        var newRatingCount = allReviews.Count;
        var newAverageRating = allReviews.Average(x => x.Rating);
        
        // update product 
        var product = await _unitOfWork.Products.GetProductByIdAsync(request.ProductId);
        if (product != null)
        {
            product.ReviewCount = newRatingCount;
            product.AverageRating = Math.Round(newAverageRating, 1);
            
            // update 
            await _unitOfWork.Products.UpdateProductAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new ProductReviewResponse
        {
            Success = true,
            Message = "Review created successfully.",
        };
    }
}