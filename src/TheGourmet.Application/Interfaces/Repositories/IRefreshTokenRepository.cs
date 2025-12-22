using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        // Add new refresh token
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        // Get refresh token by token string
        Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);
        // Update refresh token
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    }
}