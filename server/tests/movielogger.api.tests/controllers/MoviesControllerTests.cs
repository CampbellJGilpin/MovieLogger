using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.genres;
using movielogger.api.models.responses.genres;
using movielogger.api.models.responses.movies;
using movielogger.api.tests.fixtures;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class MoviesControllerTests : BaseTestController
{
    [Fact]
    public async Task GetAllMovies_ReturnsSeededMovies()
    {
        // Arrange 
        var response = await _client.GetAsync("/movies");

        // Act
        response.EnsureSuccessStatusCode();

        // Assert
        var movies = await response.Content.ReadFromJsonAsync<List<MovieResponse>>();
        movies.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}