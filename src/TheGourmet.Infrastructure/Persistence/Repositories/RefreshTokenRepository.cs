using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TheGourmetDbContext _dbContext;
        public RefreshTokenRepository(TheGourmetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Add new refresh token
        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        // Get refresh token by token string
        public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        // Update refresh token
        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
    }
}