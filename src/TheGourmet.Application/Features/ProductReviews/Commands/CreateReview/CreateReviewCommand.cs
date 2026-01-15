using System.ComponentModel.DataAnnotations;
using MediatR;
using TheGourmet.Application.Features.ProductReviews.Results;

namespace TheGourmet.Application.Features.ProductReviews.Commands.CreateReview;

public class CreateReviewCommand : IRequest<ProductReviewResponse>
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Guid OrderId { get; set; }
    
    [Range(1, 5)]
    public int Rating { get; set; } // e.g., 1 to 5
    public string? Comments { get; set; }
}