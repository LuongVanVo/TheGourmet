using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TheGourmet.Application.DTOs.Cart;
using TheGourmet.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly IDistributedCache _redisCache;
    private readonly TheGourmetDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public CartRepository(IDistributedCache redisCache, TheGourmetDbContext dbContext, IMapper mapper, IServiceScopeFactory scopeFactory)
    {
        _redisCache = redisCache;
        _dbContext = dbContext;
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }
    
    // Get cart by user ID
    public async Task<CartDto?> GetCartAsync(Guid userId)
    {
        var key = GetRedisKey(userId);
        
        // Try to get cart from Redis cache primarily
        var redisData = await _redisCache.GetStringAsync(key);
        if (!string.IsNullOrWhiteSpace(redisData))
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true 
            };
            return JsonSerializer.Deserialize<CartDto>(redisData, jsonOptions);
        }
        
        // If not found in cache, move to get from database sql
        var dbCart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (dbCart != null)
        {
            var cartDto = _mapper.Map<CartDto>(dbCart);
            // Save to Redis cache for future requests
            await SaveToRedisAsync(userId, cartDto);
            return cartDto;
        }
        
        return new CartDto
        {
            UserId = userId
        };
    }
    
    // update cart
    public async Task UpdateCartAsync(Guid userId, CartDto cartDto)
    {
        // save to redis
        await SaveToRedisAsync(userId, cartDto);
        
        // sync data to database
        _ = Task.Run(async () =>
        {
            await SyncToPostgresBackground(userId, cartDto);
        });
    }
    
    // delete cart
    public async Task DeleteCartAsync(Guid userId)
    {
        var key = GetRedisKey(userId);
        
        // remove from redis
        await _redisCache.RemoveAsync(key);
        
        // remove from database
        var dbCart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (dbCart != null)
        {
            _dbContext.Carts.Remove(dbCart);
            await _dbContext.SaveChangesAsync();
        }
    }
    
    // Helper function
    // Get Redis key for cart
    private string GetRedisKey(Guid userId) => $"cart:{userId}";
    
    // Save cart to Redis cache
    private async Task SaveToRedisAsync(Guid userId, CartDto cartDto)
    {
        var key = GetRedisKey(userId);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var data = JsonSerializer.Serialize(cartDto, jsonOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(60) // Cart expires in 60 days
        };

        await _redisCache.SetStringAsync(key, data, options);
    }
    
    // Sync from Redis to Database
    private async Task SyncToPostgresBackground(Guid userId, CartDto cartDto)
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TheGourmetDbContext>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                var dbCart = await dbContext.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (dbCart == null)
                {
                    dbCart = new Cart { UserId = userId };
                    dbContext.Carts.Add(dbCart);
                }
                else
                {
                    dbContext.CartItems.RemoveRange(dbCart.Items);
                }

                foreach (var itemDto in cartDto.Items)
                {
                    var cartItem = mapper.Map<CartItem>(itemDto);
                    cartItem.CartId = dbCart.Id;
                    dbCart.Items.Add(cartItem);
                }

                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing cart to Postgres: {ex.Message}");
        }
    }
}