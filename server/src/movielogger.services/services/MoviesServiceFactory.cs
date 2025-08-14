using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class MoviesServiceFactory
{
    private readonly IMoviesService _baseMoviesService;
    private readonly CachedMoviesService _cachedMoviesService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MoviesServiceFactory> _logger;

    public MoviesServiceFactory(
        IMoviesService baseMoviesService,
        CachedMoviesService cachedMoviesService,
        IConfiguration configuration,
        ILogger<MoviesServiceFactory> logger)
    {
        _baseMoviesService = baseMoviesService;
        _cachedMoviesService = cachedMoviesService;
        _configuration = configuration;
        _logger = logger;
    }

    public IMoviesService Create()
    {
        var useCaching = _configuration.GetValue<bool>("Caching:EnableMoviesCaching", true);

        if (useCaching)
        {
            _logger.LogInformation("Using cached MoviesService");
            return _cachedMoviesService;
        }

        _logger.LogInformation("Using non-cached MoviesService");
        return _baseMoviesService;
    }
}