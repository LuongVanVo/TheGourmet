using AutoMapper;
using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Vouchers.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Vouchers.Commands.UpdateVoucher;

public class UpdateVoucherHandler : IRequestHandler<UpdateVoucherCommand, VoucherResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public UpdateVoucherHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<VoucherResponse> Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _unitOfWork.Vouchers.GetByIdAsync(request.Id);
        if (voucher == null) 
            throw new BadRequestException("Voucher not found");
        
        // Partial update voucher
        if (request.Code != null)
        {
            var existingVoucher = await _unitOfWork.Vouchers.GetByCodeAsync(request.Code.Trim().ToUpper());
            if (existingVoucher != null && existingVoucher.Id != request.Id) 
            {
                throw new BadRequestException("Voucher code already exists");
            }
            else
            {
                voucher.Code = request.Code.Trim().ToUpper();
            }
        }
        
        if (request.DiscountType.HasValue) voucher.DiscountType = request.DiscountType.Value;
        if (request.DiscountValue.HasValue) voucher.DiscountValue = request.DiscountValue.Value;
        if (request.MaxDiscountAmount.HasValue) voucher.MaxDiscountAmount = request.MaxDiscountAmount.Value;
        if (request.MinOrderAmount.HasValue) voucher.MinOrderAmount = request.MinOrderAmount.Value;
        if (request.Quantity.HasValue) voucher.Quantity = request.Quantity.Value;
        if (request.StartDate.HasValue) voucher.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) voucher.EndDate = request.EndDate.Value;
        
        if (voucher.StartDate >= voucher.EndDate)
        {
            throw new BadRequestException("Start date must be before end date");
        }
        
        await _unitOfWork.Vouchers.UpdateAsync(voucher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new VoucherResponse
        {
            Id = voucher.Id,
            Code = voucher.Code,
            Success = true,
            Message = "Voucher updated successfully"
        };
    }
}