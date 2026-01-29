using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired();
        
        // Relationships
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(o => o.Voucher)
            .WithMany(v => v.Orders)
            .HasForeignKey(o => o.VoucherId)
            .OnDelete(DeleteBehavior.SetNull); // If voucher is deleted, keep the order but set VoucherId to null
        builder.HasOne(o => o.OrderCancelReason)
            .WithMany(r => r.Orders)
            .HasForeignKey(o => o.ReasonCancelId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Index userId to load orders by user quickly
        builder.HasIndex(o => o.UserId);
    }
}