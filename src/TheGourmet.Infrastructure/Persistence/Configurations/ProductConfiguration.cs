using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade); // Nếu một Category bị xóa, tất cả Products liên quan cũng sẽ bị xóa

        builder.Property(p => p.RowVersion)
            .IsRowVersion(); // Báo hiệu đây là ConcurrencyToken (dùng cho optimistic concurrency control)
    }
}