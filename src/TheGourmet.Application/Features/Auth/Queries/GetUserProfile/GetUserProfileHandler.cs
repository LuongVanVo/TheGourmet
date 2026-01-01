using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Auth.Queries.GetUserProfile
{
    public class GetUserProfileHandler(IUserRepository userRepository)
        : IRequestHandler<GetUserProfileQuery, UserProfileRespone>
    {
        public async Task<UserProfileRespone> Handle(GetUserProfileQuery command, CancellationToken cancellationToken)
        {
            // find user by id
            var foundUser = await userRepository.GetUserProfileByIdAsync(command.UserId);
            if (foundUser == null)
            {
                throw new NotFoundException("User not found");
            }

            // get user roles
            var roles = await userRepository.GetUserRolesAsync(foundUser);

            return new UserProfileRespone
            {
                Id = foundUser.Id,
                Fullname = foundUser.Fullname,
                Email = foundUser.Email ?? string.Empty,
                Roles = roles
            };
        }
    }
}