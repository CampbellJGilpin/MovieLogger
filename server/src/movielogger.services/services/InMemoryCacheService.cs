using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using movielogger.services.interfaces;
using System.Collections.Concurrent;

namespace movielogger.services.services;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _defaultOptions;
    private readonly ConcurrentDictionary<string, object> _cacheKeys;
    private readonly ILogger<InMemoryCacheService> _logger;

    public InMemoryCacheService(IMemoryCache memoryCache, ILogger<InMemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _cacheKeys = new ConcurrentDictionary<string, object>();
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
        _cacheKeys.TryAdd(key, new object());
        _logger.LogDebug("Cache set for key: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        _cacheKeys.TryRemove(key, out _);
        _logger.LogDebug("Cache removed for key: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        _logger.LogInformation("RemoveByPatternAsync called with pattern: {Pattern}", pattern);
        _logger.LogInformation("Current cache keys: {Keys}", string.Join(", ", _cacheKeys.Keys));
        
        // Convert wildcard pattern to regex
        var regexPattern = pattern
            .Replace(".", "\\.")
            .Replace("*", ".*")
            .Replace("?", ".");
        
        var regex = new System.Text.RegularExpressions.Regex(regexPattern);
        
        var keysToRemove = _cacheKeys.Keys
            .Where(key => regex.IsMatch(key))
            .ToList();
        
        _logger.LogInformation("Keys matching pattern '{Pattern}': {Keys}", pattern, string.Join(", ", keysToRemove));
        
        foreach (var key in keysToRemove)
        {
            _memoryCache.Remove(key);
            _cacheKeys.TryRemove(key, out _);
            _logger.LogDebug("Removed cache key: {Key}", key);
        }
        
        _logger.LogInformation("Removed {Count} cache entries for pattern: {Pattern}", keysToRemove.Count, pattern);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }
} 