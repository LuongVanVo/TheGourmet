using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace TheGourmet.Application.Features.Auth.Queries.GetUserProfile
{
    public class GetUserProfileValidator : AbstractValidator<GetUserProfileQuery>
    {
        public GetUserProfileValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}