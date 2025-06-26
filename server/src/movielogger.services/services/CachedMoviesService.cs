using AutoMapper;
using Microsoft.Extensions.Logging;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class CachedMoviesService : IMoviesService
{
    private readonly IMoviesService _moviesService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedMoviesService> _logger;
    private readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(15);

    public CachedMoviesService(IMoviesService moviesService, ICacheService cacheService, ILogger<CachedMoviesService> logger)
    {
        _moviesService = moviesService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<(IEnumerable<MovieDto> Movies, int TotalCount)> GetAllMoviesAsync(int page = 1, int pageSize = 10)
    {
        var cacheKey = $"movies:all:{page}:{pageSize}";
        
        var cached = await _cacheService.GetAsync<(IEnumerable<MovieDto> Movies, int TotalCount)>(cacheKey);
        if (cached != default)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
        var result = await _moviesService.GetAllMoviesAsync(page, pageSize);
        await _cacheService.SetAsync(cacheKey, result, _defaultCacheExpiration);
        
        return result;
    }

    public async Task<MovieDto> GetMovieByIdAsync(int movieId)
    {
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
        
        return result;
    }

    public async Task<MovieDto> CreateMovieAsync(MovieDto dto)
    {
        var result = await _moviesService.CreateMovieAsync(dto);
        
        // Invalidate related caches
        await InvalidateMovieCaches();
        
        return result;
    }

    public async Task<MovieDto> UpdateMovieAsync(int movieId, MovieDto dto)
    {
        var result = await _moviesService.UpdateMovieAsync(movieId, dto);
        
        // Invalidate specific movie cache and related caches
        await _cacheService.RemoveAsync($"movie:{movieId}");
        await InvalidateMovieCaches();
        
        return result;
    }

    public async Task<bool> DeleteMovieAsync(int movieId)
    {
        var result = await _moviesService.DeleteMovieAsync(movieId);
        
        if (result)
        {
            // Invalidate specific movie cache and related caches
            await _cacheService.RemoveAsync($"movie:{movieId}");
            await InvalidateMovieCaches();
        }
        
        return result;
    }

    public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string query)
    {
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
        
        return result;
    }

    public async Task<(IEnumerable<UserMovieDto> Items, int TotalCount, int TotalPages)> GetAllMoviesForUserAsync(int userId, string? search = null, int page = 1, int pageSize = 10)
    {
        var searchParam = search ?? "all";
        var cacheKey = $"movies:user:{userId}:{searchParam}:{page}:{pageSize}";
        
        var cached = await _cacheService.GetAsync<(IEnumerable<UserMovieDto> Items, int TotalCount, int TotalPages)>(cacheKey);
        if (cached != default)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
        var result = await _moviesService.GetAllMoviesForUserAsync(userId, search, page, pageSize);
        await _cacheService.SetAsync(cacheKey, result, _defaultCacheExpiration);
        
        return result;
    }

    private async Task InvalidateMovieCaches()
    {
        // Remove all movie-related caches
        await _cacheService.RemoveByPatternAsync("movies:*");
        await _cacheService.RemoveByPatternAsync("movie:*");
    }
} 