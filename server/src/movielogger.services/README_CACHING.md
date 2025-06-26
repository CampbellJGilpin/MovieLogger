# Caching Implementation

This document explains the caching implementation in the MovieLogger application, which follows the Dependency Inversion Principle (DIP) for easy cache provider switching.

## Architecture Overview

The caching implementation uses the **Decorator Pattern** and **Dependency Inversion Principle** to provide a flexible, swappable caching solution.

### Key Components

1. **ICacheService** - Interface defining cache operations
2. **InMemoryCacheService** - In-memory cache implementation using Microsoft.Extensions.Caching.Memory
3. **RedisCacheService** - Redis cache implementation using StackExchange.Redis
4. **CachedMoviesService** - Decorator that adds caching to IMoviesService
5. **MoviesServiceFactory** - Factory for creating cached or non-cached service instances
6. **CacheConfigurationService** - Configuration service for cache settings

## How It Works

### 1. Cache Service Interface (DIP Compliance)

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
}
```

### 2. Decorator Pattern for Service Caching

The `CachedMoviesService` wraps the original `MoviesService` and adds caching behavior:

```csharp
public class CachedMoviesService : IMoviesService
{
    private readonly IMoviesService _moviesService;
    private readonly ICacheService _cacheService;
    
    // Implements all IMoviesService methods with caching
}
```

### 3. Factory Pattern for Easy Switching

The `MoviesServiceFactory` allows easy switching between cached and non-cached implementations:

```csharp
public IMoviesService Create()
{
    var useCaching = _configuration.GetValue<bool>("Caching:EnableMoviesCaching", true);
    return useCaching ? _cachedMoviesService : _baseMoviesService;
}
```

## Configuration

### appsettings.json

```json
{
  "Caching": {
    "Enabled": true,
    "Provider": "Memory",
    "EnableMoviesCaching": true,
    "DefaultExpirationMinutes": 15,
    "SlidingExpirationMinutes": 30,
    "Redis": {
      "ConnectionString": "localhost:6379"
    }
  }
}
```

### Configuration Options

- **Enabled**: Master switch for all caching
- **Provider**: Cache provider ("Memory" or "Redis")
- **EnableMoviesCaching**: Enable/disable caching for MoviesService specifically
- **DefaultExpirationMinutes**: Default cache expiration time
- **SlidingExpirationMinutes**: Sliding expiration time for memory cache
- **Redis.ConnectionString**: Redis connection string (when using Redis)

## Usage Examples

### Switching Cache Providers

To switch from in-memory to Redis caching:

1. Update `appsettings.json`:
```json
{
  "Caching": {
    "Provider": "Redis",
    "Redis": {
      "ConnectionString": "your-redis-connection-string"
    }
  }
}
```

2. Update `Program.cs` service registration:
```csharp
// For Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(configuration.GetValue<string>("Caching:Redis:ConnectionString")));
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// For Memory (default)
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, InMemoryCacheService>();
```

### Disabling Caching

To disable caching entirely:

```json
{
  "Caching": {
    "Enabled": false
  }
}
```

Or disable just for movies:

```json
{
  "Caching": {
    "EnableMoviesCaching": false
  }
}
```

## Cache Keys

The implementation uses consistent cache key patterns:

- `movies:all:{page}:{pageSize}` - All movies with pagination
- `movie:{movieId}` - Individual movie by ID
- `movies:search:{query}` - Search results
- `movies:user:{userId}:{search}:{page}:{pageSize}` - User-specific movie lists

## Cache Invalidation

The `CachedMoviesService` automatically invalidates related caches when data changes:

- **Create/Update/Delete**: Invalidates all movie-related caches
- **Update**: Also invalidates specific movie cache
- **Delete**: Also invalidates specific movie cache

## Benefits

### 1. Performance
- Reduces database queries for frequently accessed data
- Improves response times for movie listings and searches
- Reduces database load

### 2. Scalability
- Easy to switch between cache providers
- Can scale horizontally with Redis
- Configurable cache expiration

### 3. Maintainability
- Follows DIP principles
- Easy to test with mock cache services
- Clear separation of concerns

### 4. Flexibility
- Can enable/disable caching per service
- Configurable cache providers
- Environment-specific configurations

## Testing

### Unit Testing with Mock Cache

```csharp
[Fact]
public async Task GetMovieByIdAsync_WithCacheHit_ReturnsCachedData()
{
    // Arrange
    var mockCache = Substitute.For<ICacheService>();
    var cachedMovie = new MovieDto { Id = 1, Title = "Test Movie" };
    mockCache.GetAsync<MovieDto>("movie:1").Returns(cachedMovie);
    
    var service = new CachedMoviesService(_moviesService, mockCache, _logger);
    
    // Act
    var result = await service.GetMovieByIdAsync(1);
    
    // Assert
    result.Should().BeEquivalentTo(cachedMovie);
    await _moviesService.DidNotReceive().GetMovieByIdAsync(1);
}
```

## Future Enhancements

1. **Distributed Caching**: Add support for distributed cache providers
2. **Cache Warming**: Pre-populate cache with frequently accessed data
3. **Cache Analytics**: Monitor cache hit/miss ratios
4. **Cache Compression**: Compress cached data for memory efficiency
5. **Cache Tags**: Support for tag-based cache invalidation

## Dependencies

### Required Packages

For in-memory caching:
- `Microsoft.Extensions.Caching.Memory`

For Redis caching:
- `StackExchange.Redis`

### Optional Packages

For advanced features:
- `Microsoft.Extensions.Caching.StackExchangeRedis` (for Redis health checks) 