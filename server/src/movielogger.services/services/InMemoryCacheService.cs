using Microsoft.Extensions.Caching.Memory;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _defaultOptions;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _defaultOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(30),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        var result = _memoryCache.Get<T>(key);
        return Task.FromResult(result);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = expiration.HasValue 
            ? new MemoryCacheEntryOptions
            {
                SlidingExpiration = expiration.Value,
                AbsoluteExpirationRelativeToNow = expiration.Value.Add(TimeSpan.FromMinutes(30))
            }
            : _defaultOptions;

        _memoryCache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // Note: IMemoryCache doesn't support pattern-based removal out of the box
        // This is a limitation of the in-memory cache. For production, consider Redis
        // For now, we'll log this limitation
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }
} 