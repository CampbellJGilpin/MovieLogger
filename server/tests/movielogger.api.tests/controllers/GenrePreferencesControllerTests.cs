using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.responses.genres;
using movielogger.api.tests.fixtures;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

[Collection("IntegrationTests")]
public class GenrePreferencesControllerTests : BaseTestController
{
    [Fact]
    public async Task GetUserGenrePreferences_WithValidToken_ReturnsGenrePreferences()
    {
        // Act
        var response = await _client.GetAsync("/api/genre-preferences/summary");

        // Assert
        response.EnsureSuccessStatusCode();
        var preferences = await response.Content.ReadFromJsonAsync<GenrePreferencesSummaryResponse>();

        preferences.Should().NotBeNull();
        preferences!.UserId.Should().BeGreaterThan(0);
        preferences.TotalMoviesWatched.Should().BeGreaterThanOrEqualTo(0);
        preferences.TotalUniqueGenres.Should().BeGreaterThanOrEqualTo(0);
        preferences.GenrePreferences.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserGenrePreferences_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var clientWithoutAuth = _factory.CreateClient();

        // Act
        var response = await clientWithoutAuth.GetAsync("/api/genre-preferences/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTopGenresByWatchCount_WithValidToken_ReturnsTopGenres()
    {
        // Act
        var response = await _client.GetAsync("/api/genre-preferences/top-by-watches?count=3");

        // Assert
        response.EnsureSuccessStatusCode();
        var genres = await response.Content.ReadFromJsonAsync<List<GenrePreferenceResponse>>();

        genres.Should().NotBeNull();
        genres!.Should().HaveCountLessThanOrEqualTo(3);

        if (genres.Any())
        {
            genres.Should().BeInDescendingOrder(g => g.WatchCount);
        }
    }

    [Fact]
    public async Task GetTopGenresByWatchCount_WithInvalidCount_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/genre-preferences/top-by-watches?count=-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTopGenresByRating_WithValidToken_ReturnsTopRatedGenres()
    {
        // Act
        var response = await _client.GetAsync("/api/genre-preferences/top-by-rating?count=3");

        // Assert
        response.EnsureSuccessStatusCode();
        var genres = await response.Content.ReadFromJsonAsync<List<GenrePreferenceResponse>>();

        genres.Should().NotBeNull();
        genres!.Should().HaveCountLessThanOrEqualTo(3);

        if (genres.Any())
        {
            genres.Should().BeInDescendingOrder(g => g.AverageRating);
        }
    }

    [Fact]
    public async Task GetLeastWatchedGenres_WithValidToken_ReturnsLeastWatchedGenres()
    {
        // Act
        var response = await _client.GetAsync("/api/genre-preferences/least-watched?count=3");

        // Assert
        response.EnsureSuccessStatusCode();
        var genres = await response.Content.ReadFromJsonAsync<List<GenrePreferenceResponse>>();

        genres.Should().NotBeNull();
        genres!.Should().HaveCountLessThanOrEqualTo(3);

        if (genres.Any())
        {
            genres.Should().BeInAscendingOrder(g => g.WatchCount);
        }
    }

    [Fact]
    public async Task GetGenreWatchTrends_WithValidToken_ReturnsTrends()
    {
        // Act
        var response = await _client.GetAsync("/api/genre-preferences/trends?months=6");

        // Assert
        response.EnsureSuccessStatusCode();
        var trends = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();

        trends.Should().NotBeNull();
        trends!.Should().BeOfType<Dictionary<string, int>>();
    }

    [Fact]
    public async Task GetGenreWatchTrends_WithInvalidMonths_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/genre-preferences/trends?months=-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetGenreWatchTrends_WithLargeMonths_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/genre-preferences/trends?months=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}