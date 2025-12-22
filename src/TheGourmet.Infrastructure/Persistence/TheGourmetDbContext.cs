using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Infrastructure.Persistence;

public class TheGourmetDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public TheGourmetDbContext(DbContextOptions<TheGourmetDbContext> options) : base(options)
    {
        
    }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Load các config khác (nếu có)
        builder.ApplyConfigurationsFromAssembly(typeof(TheGourmetDbContext).Assembly);

        // Customize Identity table names
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");

        // Liên kết User và Role với bảng trung gian UserRoles
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        // Lưu trữ các quyền của User trong bảng UserClaims (Ví dụ: Admin có quyền gì, User có quyền gì)
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");

        // Các bảng khác của identity (nếu cần)
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}