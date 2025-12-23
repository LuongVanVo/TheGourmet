using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using TheGourmet.Application.Common.Events;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;

        public ForgotPasswordHandler(IUserRepository userRepository, IConfiguration configuration, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;

        }

        public async Task<AuthResponse> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null || !await _userRepository.IsEmailConfirmedAsync(user)) 
                throw new NotFoundException("User not found");

            var token = await _userRepository.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{_configuration["ClientUrl"]}/api/auth/reset-password-page?email={Uri.EscapeDataString(command.Email)}&token={Uri.EscapeDataString(token)}";

            await _publishEndpoint.Publish(new ForgotPasswordEvent
            {
                Email = command.Email,
                ResetLink = resetLink
            });

            return new AuthResponse
            {
                Success = true,
                Message = "Check your email to reset your password"
            };
        }
    }
}