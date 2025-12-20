using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.Fullname) 
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.Email) 
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(u => u.Email).IsUnique(); // Đảm bảo email là duy nhất
    }
}