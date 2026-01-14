using TheGourmet.Application.DTOs.Order;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Infrastructure.Services;

public class OrderCalculationService : IOrderCalculationService
{
    private readonly IUnitOfWork _unitOfWork;
    public OrderCalculationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderPreviewDto> CalculateOrderAsync(List<CreateOrderItemDto> items, string? voucherCode)
    {
        decimal subTotal = 0;

        foreach (var item in items)
        {
            var product = await _unitOfWork.Products.GetProductByIdAsync(item.ProductId);
            if (product == null) throw new NotFoundException($"Product with ID {item.ProductId} not found.");

            subTotal += product.Price * item.Quantity;
        }
        
        // Calculate discount if voucher code is provided
        decimal discountAmount = 0;
        Guid? voucherId = null;
        string message = "Không áp dụng mã giảm giá";

        if (!string.IsNullOrEmpty(voucherCode))
        {
            var voucher = await _unitOfWork.Vouchers.GetByCodeAsync(voucherCode);
            if (voucher == null)
                throw new BadRequestException("Voucher code is invalid.");
            
            if (voucher.Quantity <= 0) 
                throw new BadRequestException("Voucher is out of stock.");

            var now = DateTime.UtcNow;
            if (now < voucher.StartDate || now > voucher.EndDate) 
                throw new BadRequestException("Voucher is not valid at this time.");
            
            if (subTotal < voucher.MinOrderAmount)
                throw new BadRequestException("Order amount does not meet the minimum requirement for this voucher.");
            
            // calculate
            if (voucher.DiscountType == DiscountType.FixedAmount)
            {
                discountAmount = voucher.DiscountValue;
            }
            else
            {
                discountAmount = subTotal * (voucher.DiscountValue / 100);
                if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                {
                    discountAmount = voucher.MaxDiscountAmount.Value;
                }
            }
            // discount không được vượt quá tổng tiền
            if (discountAmount > subTotal) discountAmount = subTotal;

            voucherId = voucher.Id;
            message = "Áp dụng mã giảm giá thành công!";
        }
        
        // Calculate shipping fee
        decimal shippingFee = (subTotal > 500000) ? subTotal * 0.02m : subTotal * 0.05m;

        return new OrderPreviewDto
        {
            SubTotal = subTotal,
            ShippingFee = shippingFee,
            DiscountAmount = discountAmount,
            TotalAmount = subTotal + shippingFee - discountAmount,
            ApplidVoucherCode = voucherCode,
            VoucherId = voucherId,
            Message = message
        };
    }
}