using AutoMapper;
using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Vouchers.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Vouchers.Commands.CreateVoucher;

public class CreateVoucherHandler : IRequestHandler<CreateVoucherCommand, VoucherResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CreateVoucherHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<VoucherResponse> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        // Validate date
        if (request.StartDate >= request.EndDate)
        {
            throw new BadRequestException("Start date must be before end date");
        }
        
        // Check voucher code already exists
        var existingVoucher = await _unitOfWork.Vouchers.GetByCodeAsync(request.Code.Trim().ToUpper());
        if (existingVoucher != null) 
        {
            throw new BadRequestException("Voucher code already exists");
        }
        
        // Create new voucher
        var voucher = _mapper.Map<Voucher>(request);

        await _unitOfWork.Vouchers.AddAsync(voucher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new VoucherResponse
        {
            Id = voucher.Id,
            Code = voucher.Code,
            Success = true,
            Message = "Voucher created successfully"
        };
    }
}