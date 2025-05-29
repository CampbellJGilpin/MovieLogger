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
        var response = await _client.GetAsync("/genres");

        // Assert
        response.EnsureSuccessStatusCode();

        var genres = await response.Content.ReadFromJsonAsync<List<GenreResponse>>();
        genres.Should().HaveCountGreaterThanOrEqualTo(2);
    }
    
    [Fact]
    public async Task GetGenreById_ReturnsCorrectGenre()
    {
        // Act
        var singleGenreResponse = await _client.GetAsync($"/genres/1");
        
        // Assert
        singleGenreResponse.EnsureSuccessStatusCode();

        var genre = await singleGenreResponse.Content.ReadFromJsonAsync<GenreResponse>();
        genre.Should().NotBeNull();
        genre!.Id.Should().Be(1);
        genre.Title.Should().Be("Horror");
    }

    [Fact]
    public async Task CreateGenre_ReturnsCreatedGenre()
    {
        // Arrange
        var request = new CreateGenreRequest { Title = "Musical" };
        
        // Act
        var response = await _client.PostAsJsonAsync("/genres", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdGenre = await response.Content.ReadFromJsonAsync<GenreResponse>();
        
        createdGenre.Should().NotBeNull();
        createdGenre.Title.Should().Be("Musical");
        createdGenre.Id.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task UpdateGenre_ReturnsUpdatedGenre()
    {
        // Arrange
        var request = new UpdateGenreRequest { Title = "Super Horror" };
        
        // Act
        var response = await _client.PutAsJsonAsync("/genres/1", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdGenre = await response.Content.ReadFromJsonAsync<GenreResponse>();
        
        createdGenre.Should().NotBeNull();
        createdGenre.Title.Should().Be("Super Horror");
        createdGenre.Id.Should().Be(1);
    }
}