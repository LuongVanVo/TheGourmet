using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // set table name
        builder.ToTable("Categories");
        
        // Configure properties
        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired(); // Name không được để trống

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.ImageUrl)
            .IsRequired(false);
        
        // Configure indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();
    }
}