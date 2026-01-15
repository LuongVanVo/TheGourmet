using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");
        
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Rating).IsRequired();
        builder.Property(pr => pr.Comments).HasMaxLength(1000);
        
        builder.HasOne(pr => pr.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(pr => pr.User) 
            .WithMany() // User can review many products
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Order>() 
            .WithMany()
            .HasForeignKey(pr => pr.OrderId)
            .OnDelete(DeleteBehavior.NoAction); 
        
        // Ensure a user can only review a product once per order
        builder.HasIndex(pr => new { pr.OrderId, pr.ProductId }).IsUnique();
    }
}