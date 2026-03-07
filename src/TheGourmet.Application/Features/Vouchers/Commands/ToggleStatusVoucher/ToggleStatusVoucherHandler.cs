using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Vouchers.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Vouchers.Commands.ToggleStatusVoucher;

public class ToggleStatusVoucherHandler : IRequestHandler<ToggleStatusVoucherCommand, VoucherResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public ToggleStatusVoucherHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VoucherResponse> Handle(ToggleStatusVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _unitOfWork.Vouchers.GetByIdAsync(request.Id);
        if (voucher == null)
            throw new BadRequestException("Voucher not found");
        
        voucher.IsActive = !voucher.IsActive;
        voucher.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Vouchers.UpdateAsync(voucher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new VoucherResponse
        {
            Id = voucher.Id,
            Code = voucher.Code,
            Success = true,
            Message = $"Voucher {(voucher.IsActive ? "activated" : "inactivated")} successfully"
        };
    }
}