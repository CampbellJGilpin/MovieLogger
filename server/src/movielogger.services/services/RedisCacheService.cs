using Microsoft.Extensions.Logging;
using movielogger.services.interfaces;
using StackExchange.Redis;

namespace movielogger.services.services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
        _database = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (!value.HasValue)
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<T>(value!);
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from Redis for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            var serializedValue = System.Text.Json.JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, serializedValue, expiration);
            _logger.LogDebug("Cache set for key: {Key} with expiration: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in Redis for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            var removed = await _database.KeyDeleteAsync(key);
            _logger.LogDebug("Cache removed for key: {Key}, success: {Success}", key, removed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key from Redis: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            _logger.LogInformation("RemoveByPatternAsync called with pattern: {Pattern}", pattern);
            
            // Get all endpoints (Redis can have multiple servers)
            var endpoints = _redis.GetEndPoints();
            var totalRemoved = 0;
            
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                
                // Use KEYS for pattern matching (not recommended for huge datasets, but fine for most app caches)
                var keys = server.Keys(pattern: pattern).ToArray();
                
                _logger.LogInformation("Found {Count} keys matching pattern '{Pattern}' on endpoint {Endpoint}", 
                    keys.Length, pattern, endpoint);
                
                // Delete keys in batches for better performance
                if (keys.Length > 0)
                {
                    var batch = _database.CreateBatch();
                    var tasks = keys.Select(key => batch.KeyDeleteAsync(key)).ToArray();
                    batch.Execute();
                    await Task.WhenAll(tasks);
                    
                    totalRemoved += keys.Length;
                    _logger.LogDebug("Removed {Count} keys from endpoint {Endpoint}", keys.Length, endpoint);
                }
            }
            
            _logger.LogInformation("Pattern-based cache removal completed - removed {TotalCount} keys for pattern: {Pattern}", 
                totalRemoved, pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing keys by pattern from Redis: {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var exists = await _database.KeyExistsAsync(key);
            _logger.LogDebug("Cache exists check for key: {Key}, exists: {Exists}", key, exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if key exists in Redis: {Key}", key);
            return false;
        }
    }
} 