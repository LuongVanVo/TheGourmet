using AutoMapper;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Products.Profiles;

public class GetProductByIdProfile : Profile
{
    public GetProductByIdProfile()
    {
        CreateMap<Product, GetProductByIdResponse>()
            .ForMember(dest => dest.CategoryName, 
                opt => opt.MapFrom(src => src.Category.Name));
    }
}