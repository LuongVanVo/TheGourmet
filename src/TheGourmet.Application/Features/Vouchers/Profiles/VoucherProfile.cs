using AutoMapper;
using TheGourmet.Application.DTOs.Voucher;
using TheGourmet.Application.Features.Vouchers.Commands.CreateVoucher;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Vouchers.Profiles;

public class VoucherProfile : Profile
{
    public VoucherProfile()
    {
        CreateMap<CreateVoucherCommand, Voucher>()
            .ForMember(dest => dest.Code,
                opt => opt.MapFrom(src => src.Code.Trim().ToUpper()));
        
        // Map VoucherDto to Voucher
        CreateMap<Voucher, VoucherDto>();
    }
}