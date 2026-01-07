using AutoMapper;
using TheGourmet.Application.Features.Products.Commands.UpdateProduct;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Products.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<UpdateProductCommand, Product>() 
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) =>
            {
                // Only map non-null and non-default values
                if (srcMember == null) return false;
                
                // if srcMember is string, check for empty
                if (srcMember is string strValue)
                {
                    return !string.IsNullOrWhiteSpace(strValue);
                }
                
                // If srcMember is value type, check hasValue
                var memberType = srcMember.GetType();
                if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return srcMember != null && (bool)memberType.GetProperty("HasValue")!.GetValue(srcMember)!;
                } 
                
                return true;
            }));
        
        
    }
}