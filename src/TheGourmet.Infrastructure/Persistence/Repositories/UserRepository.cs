using Microsoft.AspNetCore.Identity;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Find user by email
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    // Create new user
    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return result; // Succeeded trả về true nếu tạo thành công
    }

    // Check password
    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    // Generate email confirmation token
    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }
    // Find user by id
    public async Task<ApplicationUser?> GetUserByUserId(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    // Confirm email
    public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result;
    }

    // Update user
    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }
    // Get all roles of user
    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }
}
