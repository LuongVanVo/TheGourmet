using Microsoft.AspNetCore.Identity;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IUserRepository
{
    // Find User by email
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    // Find user by id 
    Task<ApplicationUser?> GetUserByUserId(string userId);
    // create new user
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    // Check password
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

    // Generate email confirmation token
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    // Confirm email
    Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
    // Update user
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    // Get all roles of user
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    // Reset password
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
    // generate password reset token
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    // check isEmailConfirm
    Task<bool> IsEmailConfirmedAsync(ApplicationUser user);

    // Get user profile by id
    Task<ApplicationUser?> GetUserProfileByIdAsync(string userId);
}