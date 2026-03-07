using MediatR;
using TheGourmet.Application.Features.Vouchers.Results;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Vouchers.Commands.CreateVoucher;

public class CreateVoucherCommand : IRequest<VoucherResponse>
{
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; } // Applicable only for percentage discounts
    public decimal? MinOrderAmount { get; set; } // 
    public int Quantity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}