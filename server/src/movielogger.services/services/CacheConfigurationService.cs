using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class CacheConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CacheConfigurationService> _logger;

    public CacheConfigurationService(IConfiguration configuration, ILogger<CacheConfigurationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GetCacheProvider()
    {
        return _configuration.GetValue<string>("Caching:Provider", "Memory");
    }

    public bool IsCachingEnabled()
    {
        return _configuration.GetValue<bool>("Caching:Enabled", true);
    }

    public TimeSpan GetDefaultExpiration()
    {
        var minutes = _configuration.GetValue<int>("Caching:DefaultExpirationMinutes", 15);
        return TimeSpan.FromMinutes(minutes);
    }

    public TimeSpan GetSlidingExpiration()
    {
        var minutes = _configuration.GetValue<int>("Caching:SlidingExpirationMinutes", 30);
        return TimeSpan.FromMinutes(minutes);
    }

    public string GetRedisConnectionString()
    {
        return _configuration.GetValue<string>("Caching:Redis:ConnectionString", "localhost:6379");
    }
} 