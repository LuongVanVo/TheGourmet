using MediatR;
using TheGourmet.Application.Features.Vouchers.Results;

namespace TheGourmet.Application.Features.Vouchers.Commands.ToggleStatusVoucher;

public class ToggleStatusVoucherCommand : IRequest<VoucherResponse>
{
    public Guid Id { get; set; }
}