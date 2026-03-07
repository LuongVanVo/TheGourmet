using AutoMapper;
using MediatR;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.DTOs.Voucher;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Vouchers.Queries.GetVouchers;

public class GetVouchersHandler : IRequestHandler<GetVouchersQuery, PaginatedList<VoucherDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetVouchersHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<VoucherDto>> Handle(GetVouchersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Vouchers.GetVouchersWithPaginationAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.IsActive,
            cancellationToken
        );
        
        var mappedItems = _mapper.Map<List<VoucherDto>>(items);
        
        return new PaginatedList<VoucherDto>(mappedItems, totalCount, request.PageNumber, request.PageSize);
    }
}