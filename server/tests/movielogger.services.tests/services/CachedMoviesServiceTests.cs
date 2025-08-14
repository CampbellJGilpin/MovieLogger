using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using movielogger.dal.dtos;
using movielogger.services.interfaces;
using movielogger.services.services;
using NSubstitute;
using Xunit;
using System.Collections.Generic;

namespace movielogger.services.tests.services;

public class CachedMoviesServiceTests
{
    private readonly IFixture _fixture;
    private readonly IMoviesService _baseMoviesService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedMoviesService> _logger;
    private readonly IConfiguration _configuration;
    private readonly CachedMoviesService _cachedService;

    public CachedMoviesServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _baseMoviesService = Substitute.For<IMoviesService>();
        _cacheService = Substitute.For<ICacheService>();
        _logger = Substitute.For<ILogger<CachedMoviesService>>();

        // Create a real configuration with the required values
        var configValues = new Dictionary<string, string?>
        {
            {"Caching:EnableMoviesCaching", "true"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        _cachedService = new CachedMoviesService(_baseMoviesService, _cacheService, _logger, _configuration);
    }

    [Fact]
    public async Task GetMovieByIdAsync_WithCacheHit_ReturnsCachedData()
    {
        // Arrange
        var movieId = 1;
        var cachedMovie = _fixture.Create<MovieDto>();
        _cacheService.GetAsync<MovieDto>($"movie:{movieId}").Returns(cachedMovie);

        // Act
        var result = await _cachedService.GetMovieByIdAsync(movieId);

        // Assert
        result.Should().BeEquivalentTo(cachedMovie);
        await _baseMoviesService.DidNotReceive().GetMovieByIdAsync(movieId);
    }

    [Fact]
    public async Task GetMovieByIdAsync_WithCacheMiss_CallsBaseServiceAndCachesResult()
    {
        // Arrange
        var movieId = 1;
        var movie = _fixture.Create<MovieDto>();
        _cacheService.GetAsync<MovieDto>($"movie:{movieId}").Returns((MovieDto?)null);
        _baseMoviesService.GetMovieByIdAsync(movieId).Returns(movie);

        // Act
        var result = await _cachedService.GetMovieByIdAsync(movieId);

        // Assert
        result.Should().BeEquivalentTo(movie);
        await _baseMoviesService.Received(1).GetMovieByIdAsync(movieId);
        await _cacheService.Received(1).SetAsync($"movie:{movieId}", movie, Arg.Any<TimeSpan>());
    }
}