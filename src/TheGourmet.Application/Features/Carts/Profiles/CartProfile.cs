using AutoMapper;
using TheGourmet.Application.DTOs.Cart;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Carts.Profiles;

public class CartProfile : Profile
{
    public CartProfile()
    {
        // Map from CartItem Entity -> CartItemDto
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImage,
                opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.StockRemaining,
                opt => opt.MapFrom(src => src.Product.StockQuantity));
        
        // Map from Cart Entity -> CartDto
        CreateMap<Cart, CartDto>();
        
        // Map Reverse from DTO -> Entity (use when sync from Redis to PostgreSQL)
        CreateMap<CartItemDto, CartItem>();
        CreateMap<CartDto, Cart>();
    }
}