using MediatR;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.DTOs.Voucher;

namespace TheGourmet.Application.Features.Vouchers.Queries.GetVouchers;

public class GetVouchersQuery : IRequest<PaginatedList<VoucherDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Option to search by code or name
    public string? SearchTerm { get; set; }
    
    // Filter by status (can get all if null)
    public bool? IsActive { get; set; }
}