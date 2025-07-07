using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.genres;
using movielogger.api.models.requests.movies;
using movielogger.api.models.responses;
using movielogger.api.models.responses.genres;
using movielogger.api.models.responses.movies;
using movielogger.api.tests.fixtures;
using movielogger.api.tests.helpers;
using movielogger.dal.dtos;
using NSubstitute;
using System.Net.Http.Headers;

namespace movielogger.api.tests.controllers;

public class MoviesControllerTests : BaseTestController
{
    private readonly new CustomWebApplicationFactory _factory;
    private readonly new HttpClient _client;

    public MoviesControllerTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetMovies_ReturnsAllMovies()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = TestDataBuilder.CreateTestUser(1);
        var token = AuthenticationHelper.GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/movies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var movies = await response.Content.ReadFromJsonAsync<PaginatedResponse<MovieResponse>>();
        movies.Should().NotBeNull();
        movies!.Items.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetMovieById_WhenMovieExists_ReturnsMovie()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = TestDataBuilder.CreateTestUser(1);
        var token = AuthenticationHelper.GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/movies/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var movie = await response.Content.ReadFromJsonAsync<MovieResponse>();
        movie.Should().NotBeNull();
        movie!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetMovieById_WhenMovieDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = TestDataBuilder.CreateTestUser(1);
        var token = AuthenticationHelper.GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/movies/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateMovie_WithValidData_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = TestDataBuilder.CreateTestUser(1);
        var token = AuthenticationHelper.GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateMovieRequest
        {
            Title = "Test Movie",
            Description = "Test Description",
            GenreId = 1,
            ReleaseDate = DateTime.Now
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/movies", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var movie = await response.Content.ReadFromJsonAsync<MovieResponse>();
        movie.Should().NotBeNull();
        movie!.Title.Should().Be("Test Movie");
    }

    [Fact]
    public async Task CreateMovie_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = TestDataBuilder.CreateTestUser(1);
        var token = AuthenticationHelper.GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateMovieRequest
        {
            Title = "", // Invalid empty title
            Description = "Test Description",
            GenreId = 1,
            ReleaseDate = DateTime.Now
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/movies", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateMovie_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = TestDataBuilder.CreateTestUser(1);
        var token = AuthenticationHelper.GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UpdateMovieRequest
        {
            Title = "Updated Movie",
            Description = "Updated Description",
            GenreId = 2,
            ReleaseDate = DateTime.Now
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/movies/1", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var movie = await response.Content.ReadFromJsonAsync<MovieResponse>();
        movie.Should().NotBeNull();
        movie!.Title.Should().Be("Updated Movie");
    }

    [Fact]
    public async Task UpdateMovie_WhenMovieDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = TestDataBuilder.CreateTestUser(1);
        var token = AuthenticationHelper.GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UpdateMovieRequest
        {
            Title = "Updated Movie",
            Description = "Updated Description",
            GenreId = 2,
            ReleaseDate = DateTime.Now
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/movies/999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}