using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        public ResetPasswordHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<AuthResponse> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null) 
                throw new NotFoundException("User not found");
            
            var result = await _userRepository.ResetPasswordAsync(user, command.Token, command.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Password reset failed: {errors}");
            }

            return new AuthResponse
            {
                Success = true,
                Message = "Password has been reset successfully"
            };
        }
    }
}