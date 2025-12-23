using MediatR;
using TheGourmet.Application.Features.Auth.Results;

namespace TheGourmet.Application.Features.Auth.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<UserProfileRespone>
    {
        public string UserId { get; set; } = string.Empty;
    }
}