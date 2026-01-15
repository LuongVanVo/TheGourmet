using AutoMapper;
using TheGourmet.Application.DTOs.ProductReview;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.ProductReviews.Profiles;

public class ProductReviewProfile : Profile
{
    public ProductReviewProfile()
    {
        CreateMap<ProductReview, ProductReviewDto>()
            .ForMember(dest => dest.ReviewerName,
                opt => opt.MapFrom(src => src.User != null ? src.User.Fullname : "Ẩn danh"))
            .ForMember(dest => dest.ReviewerAvatar,
                opt => opt.MapFrom(src => src.User != null ? src.User.AvatarUrl : null))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comments));
    }
}