using AutoMapper;
using TheGourmet.Application.Features.Categories.Commands.UpdateCategory;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Categories.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        // Map from UpdateCategoryCommand to Category (only non-null fields)
        CreateMap<UpdateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Not map Id from command
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) =>
            {
                // only map if value is not null
                if (srcMember == null) return false;
                
                // If the srcMember is string, check not null/empty
                if (srcMember is string strValue)
                {
                    return !string.IsNullOrWhiteSpace(strValue);
                }
                
                // If nullable value type, check HasValue
                var memberType = srcMember.GetType();
                if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return srcMember != null && (bool)memberType.GetProperty("HasValue")!.GetValue(srcMember)!;
                }

                return true;
            }));
    }
}