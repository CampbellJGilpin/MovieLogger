using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.genres;
using movielogger.api.models.responses.genres;
using movielogger.api.tests.fixtures;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

[Collection("IntegrationTests")]
public class GenresControllerTests : BaseTestController
{
    [Fact]
    public async Task GetAllGenres_ReturnsSeededGenres()
    {
        // Act
        var response = await _client.GetAsync("/api/genres");

        // Assert
        response.EnsureSuccessStatusCode();
        var genres = await response.Content.ReadFromJsonAsync<List<GenreResponse>>();

        genres.Should().NotBeNull();
        genres!.Should().HaveCountGreaterThanOrEqualTo(2);
        genres.Should().Contain(g => g.Title == "Horror");
        genres.Should().Contain(g => g.Title == "Action");
    }

    [Fact]
    public async Task GetGenreById_WhenGenreExists_ReturnsGenre()
    {
        // Act
        var response = await _client.GetAsync("/api/genres/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var genre = await response.Content.ReadFromJsonAsync<GenreResponse>();

        genre.Should().NotBeNull();
        genre!.Id.Should().Be(1);
        genre.Title.Should().Be("Horror");
    }

    [Fact]
    public async Task GetGenreById_WhenGenreDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/genres/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateGenre_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateGenreRequest
        {
            Title = $"TestGenre_{Guid.NewGuid().ToString()[..8]}"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/genres", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdGenre = await response.Content.ReadFromJsonAsync<GenreResponse>();

        createdGenre.Should().NotBeNull();
        createdGenre!.Title.Should().Be(request.Title);
        createdGenre.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateGenre_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateGenreRequest
        {
            Title = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/genres", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateGenre_WithDuplicateTitle_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateGenreRequest
        {
            Title = "Horror" // This genre already exists
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/genres", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateGenre_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new UpdateGenreRequest
        {
            GenreId = 1,
            Title = "Updated Horror"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/genres/1", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedGenre = await response.Content.ReadFromJsonAsync<GenreResponse>();

        updatedGenre.Should().NotBeNull();
        updatedGenre!.Title.Should().Be(request.Title);
        updatedGenre.Id.Should().Be(1);
    }

    [Fact]
    public async Task UpdateGenre_WhenGenreDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateGenreRequest
        {
            GenreId = 999,
            Title = "Non-existent Genre"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/genres/999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateGenre_WithDuplicateTitle_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateGenreRequest
        {
            GenreId = 1,
            Title = "Action" // This genre already exists
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/genres/1", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteGenre_WhenGenreExists_ReturnsSuccess()
    {
        // Arrange - Create a genre first so we can delete it
        var createRequest = new CreateGenreRequest
        {
            Title = $"ToDelete_{Guid.NewGuid().ToString()[..8]}"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/genres", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdGenre = await createResponse.Content.ReadFromJsonAsync<GenreResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/genres/{createdGenre!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify genre is deleted
        var getResponse = await _client.GetAsync($"/api/genres/{createdGenre.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteGenre_WhenGenreDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/genres/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteGenre_WhenGenreHasMovies_ReturnsBadRequest()
    {
        // Act - Try to delete a genre that has associated movies
        var response = await _client.DeleteAsync("/api/genres/1"); // Assuming genre 1 has movies

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}