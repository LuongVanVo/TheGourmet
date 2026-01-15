using MediatR;
using TheGourmet.Application.DTOs.ProductReview;

namespace TheGourmet.Application.Features.ProductReviews.Queries.GetProductReviews;

public class GetProductReviewsQuery : IRequest<List<ProductReviewDto>>
{
    public Guid ProductId { get; set; }
}