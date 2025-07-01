using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using movielogger.services.interfaces;
using System.Collections.Concurrent;

namespace movielogger.services.services;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _defaultOptions;
    private readonly ILogger<InMemoryCacheService> _logger;

    public InMemoryCacheService(IMemoryCache memoryCache, ILogger<InMemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _defaultOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(30),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        var result = _memoryCache.Get<T>(key);
        _logger.LogDebug("Cache get for key: {Key}, found: {Found}", key, result != null);
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
        _logger.LogDebug("Cache set for key: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        _logger.LogDebug("Cache removed for key: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        _logger.LogInformation("RemoveByPatternAsync called with pattern: {Pattern}", pattern);

        _logger.LogWarning("Pattern-based cache removal is not fully supported in InMemoryCache. " +
                          "Consider using RedisCacheService for production scenarios that require pattern-based cache invalidation. " +
                          "Pattern requested: {Pattern}", pattern);
        
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        _logger.LogDebug("Cache exists check for key: {Key}, exists: {Exists}", key, exists);
        return Task.FromResult(exists);
    }
} 