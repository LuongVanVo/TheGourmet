using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");
        
        // Fullname
        builder.Property(u => u.Fullname) 
            .HasMaxLength(100)
            .IsRequired();
        
        // Email
        builder.Property(u => u.Email) 
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("IX_Users_NormalizedEmail");
        
        // UserName
        builder.Property(u => u.UserName)
            .HasMaxLength(256);
        
        // Other properties
        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);
        
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);
        
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}