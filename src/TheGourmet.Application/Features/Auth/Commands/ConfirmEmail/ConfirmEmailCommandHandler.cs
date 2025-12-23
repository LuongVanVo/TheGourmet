using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        public ConfirmEmailCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<AuthResponse> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUserId(command.UserId);
            if (user == null) 
                throw new NotFoundException("User not found");
            
            // check token used
            if (user.IsActive) throw new BadRequestException("Account has already been confirmed");
            var result = await _userRepository.ConfirmEmailAsync(user, command.Token);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Email confirmation failed: {errors}");
            }

            user.IsActive = true;

            await _userRepository.UpdateUserAsync(user);

            return new AuthResponse
            {
                Message = "Email confirmed successfully",
                Success = result.Succeeded
            };
        }
    }
}