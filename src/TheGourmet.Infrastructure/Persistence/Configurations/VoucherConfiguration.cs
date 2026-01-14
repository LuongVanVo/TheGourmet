using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.ToTable("Vouchers");
        
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(v => v.Code).HasMaxLength(50).IsRequired();
        builder.Property(v => v.DiscountValue).HasColumnType("decimal(18, 2)");
    }
}