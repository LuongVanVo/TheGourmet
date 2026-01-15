namespace TheGourmet.Application.DTOs.ProductReview;

public class ProductReviewDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    
    // Infomation of Reviewer (get from User entity)
    public string ReviewerName { get; set; }
    public string? ReviewerAvatar { get; set; }
    
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Reply { get; set; } // Shop reply to the review
    
    public DateTime CreatedDate { get; set; } // Day the review was created
}