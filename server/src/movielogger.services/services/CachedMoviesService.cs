using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using movielogger.dal.dtos;
using movielogger.services.interfaces;
using movielogger.services.models;

namespace movielogger.services.services;

public class CachedMoviesService : IMoviesService
{
    private readonly IMoviesService _moviesService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedMoviesService> _logger;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(15);

    public CachedMoviesService(IMoviesService moviesService, ICacheService cacheService, ILogger<CachedMoviesService> logger, IConfiguration configuration)
    {
        _moviesService = moviesService;
        _cacheService = cacheService;
        _logger = logger;
        _configuration = configuration;
    }

    private bool IsCachingEnabled => _configuration.GetValue<bool>("Caching:EnableMoviesCaching", true);

    public async Task<(IEnumerable<MovieDto> Movies, int TotalCount)> GetAllMoviesAsync(int page = 1, int pageSize = 10)
    {
        _logger.LogInformation("GetAllMoviesAsync called - page: {Page}, pageSize: {PageSize}", page, pageSize);
        
        if (!IsCachingEnabled)
        {
            _logger.LogDebug("Caching disabled, calling base service directly");
            return await _moviesService.GetAllMoviesAsync(page, pageSize);
        }

        var cacheKey = $"movies:all:{page}:{pageSize}";
        _logger.LogDebug("Using cache key: {CacheKey}", cacheKey);
        
        var cached = await _cacheService.GetAsync<CachedMoviesResult>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return (cached.Movies, cached.TotalCount);
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
        var result = await _moviesService.GetAllMoviesAsync(page, pageSize);
        
        var cachedResult = new CachedMoviesResult
        {
            Movies = result.Movies,
            TotalCount = result.TotalCount
        };
        await _cacheService.SetAsync(cacheKey, cachedResult, _defaultCacheExpiration);
        _logger.LogInformation("Cached result for {CacheKey} with {MovieCount} movies", cacheKey, result.Movies.Count());
        
        return result;
    }

    public async Task<MovieDto> GetMovieByIdAsync(int movieId)
    {
        if (!IsCachingEnabled)
        {
            _logger.LogDebug("Caching disabled, calling base service directly");
            return await _moviesService.GetMovieByIdAsync(movieId);
        }

        var cacheKey = $"movie:{movieId}";
        
        var cached = await _cacheService.GetAsync<MovieDto>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
        var result = await _moviesService.GetMovieByIdAsync(movieId);
        await _cacheService.SetAsync(cacheKey, result, _defaultCacheExpiration);
        _logger.LogInformation("Cached individual movie for {CacheKey}: {MovieTitle}", cacheKey, result.Title);
        
        return result;
    }

    public async Task<MovieDto> CreateMovieAsync(MovieDto dto)
    {
        var result = await _moviesService.CreateMovieAsync(dto);
        
        if (IsCachingEnabled)
        {
            // Invalidate related caches
            await InvalidateMovieCaches();
        }
        
        return result;
    }

    public async Task<MovieDto> UpdateMovieAsync(int movieId, MovieDto dto)
    {
        var result = await _moviesService.UpdateMovieAsync(movieId, dto);
        
        if (IsCachingEnabled)
        {
            // Invalidate specific movie cache and related caches
            await _cacheService.RemoveAsync($"movie:{movieId}");
            await InvalidateMovieCaches();
        }
        
        return result;
    }

    public async Task<bool> DeleteMovieAsync(int movieId)
    {
        _logger.LogInformation("DeleteMovieAsync called for movie ID: {MovieId}", movieId);
        
        var result = await _moviesService.DeleteMovieAsync(movieId);
        
        if (result && IsCachingEnabled)
        {
            _logger.LogInformation("Movie {MovieId} deleted successfully, invalidating caches", movieId);
            
            // Invalidate specific movie cache and related caches
            await _cacheService.RemoveAsync($"movie:{movieId}");
            _logger.LogInformation("Removed specific movie cache for movie ID: {MovieId}", movieId);
            
            await InvalidateMovieCaches();
            _logger.LogInformation("Invalidated all movie-related caches after deletion");
        }
        else if (result)
        {
            _logger.LogInformation("Movie {MovieId} deleted successfully, caching disabled", movieId);
        }
        else
        {
            _logger.LogWarning("Failed to delete movie ID: {MovieId}, no cache invalidation needed", movieId);
        }
        
        return result;
    }

    public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string query)
    {
        if (!IsCachingEnabled)
        {
            _logger.LogDebug("Caching disabled, calling base service directly");
            return await _moviesService.SearchMoviesAsync(query);
        }

        var cacheKey = $"movies:search:{query.ToLower()}";
        
        var cached = await _cacheService.GetAsync<IEnumerable<MovieDto>>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
        var result = await _moviesService.SearchMoviesAsync(query);
        await _cacheService.SetAsync(cacheKey, result, _defaultCacheExpiration);
        _logger.LogInformation("Cached search results for {CacheKey} with {ResultCount} movies", cacheKey, result.Count());
        
        return result;
    }

    public async Task<(IEnumerable<UserMovieDto> Items, int TotalCount, int TotalPages)> GetAllMoviesForUserAsync(int userId, string? search = null, int page = 1, int pageSize = 10)
    {
        _logger.LogInformation("GetAllMoviesForUserAsync called - userId: {UserId}, search: '{Search}', page: {Page}, pageSize: {PageSize}", userId, search ?? "null", page, pageSize);
        
        if (!IsCachingEnabled)
        {
            _logger.LogDebug("Caching disabled, calling base service directly");
            return await _moviesService.GetAllMoviesForUserAsync(userId, search, page, pageSize);
        }

        var searchParam = search ?? "all";
        var cacheKey = $"movies:user:{userId}:{searchParam}:{page}:{pageSize}";
        _logger.LogDebug("Using cache key: {CacheKey}", cacheKey);
        
        var cached = await _cacheService.GetAsync<CachedUserMoviesResult>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return (cached.Items, cached.TotalCount, cached.TotalPages);
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
        var result = await _moviesService.GetAllMoviesForUserAsync(userId, search, page, pageSize);
        
        var cachedResult = new CachedUserMoviesResult
        {
            Items = result.Items,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages
        };
        await _cacheService.SetAsync(cacheKey, cachedResult, _defaultCacheExpiration);
        _logger.LogInformation("Cached user movies for {CacheKey} with {ItemCount} items", cacheKey, result.Items.Count());
        
        return result;
    }

    private async Task InvalidateMovieCaches()
    {
        _logger.LogInformation("InvalidateMovieCaches called - removing all movie-related caches");
        
        // Check if we're using Redis (which supports pattern-based removal)
        if (_cacheService is RedisCacheService)
        {
            _logger.LogInformation("Using Redis - performing pattern-based cache invalidation");
            
            // Remove all movie-related caches using patterns
            // This will automatically remove ALL user-specific caches, search caches, etc.
            await _cacheService.RemoveByPatternAsync("movies:all:*");
            await _cacheService.RemoveByPatternAsync("movies:user:*");
            await _cacheService.RemoveByPatternAsync("movies:search:*");
            await _cacheService.RemoveByPatternAsync("movie:*");
            
            _logger.LogInformation("Redis pattern-based cache invalidation completed");
        }
        else
        {
            _logger.LogInformation("Using InMemory cache - performing targeted cache invalidation");
        }
    }
} 