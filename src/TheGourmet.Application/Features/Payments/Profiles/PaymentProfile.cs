using AutoMapper;
using TheGourmet.Application.DTOs.Payment;
using TheGourmet.Domain.Entities;
using Order = StackExchange.Redis.Order;

namespace TheGourmet.Application.Features.Payments.Profiles;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Order, PaymentInformationModel>()
            .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => "Khach hang TheGourmet"))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => "other"));

        CreateMap<PaymentTransaction, PaymentInformationModel>()
            .ForMember(dest => dest.TransactionId,
                opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.OrderType,
                opt => opt.MapFrom(src => "billpayment"))
            .ForMember(dest => dest.OrderDescription,
                opt => opt.MapFrom(src => $"Payment for order {src.OrderId}"))
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => "TheGourmet Order"));

        CreateMap<PaymentTransaction, PaymentTransactionDto>()
            .ForMember(dest => dest.BankCode,
                opt => opt.MapFrom(src => src.ResponseCode))
            .ForMember(dest => dest.TransactionId,
                opt => opt.MapFrom(src => src.ProviderTransactionId));
    }
}