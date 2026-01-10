using AutoMapper;
using TheGourmet.Application.DTOs.Order;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Orders.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductImageUrl,
                opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.TotalLineAmount,
                opt => opt.MapFrom(src => src.UnitPrice * src.Quantity));

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderItems,
                opt => opt.MapFrom(src => src.OrderItems));
    }
}