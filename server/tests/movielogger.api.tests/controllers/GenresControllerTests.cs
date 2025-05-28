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
    private readonly HttpClient _client;

    public GenresControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetAllGenres_ReturnsSeededGenres()
    {
        var response = await _client.GetAsync("/genres");

        response.EnsureSuccessStatusCode();

        var genres = await response.Content.ReadFromJsonAsync<List<GenreResponse>>();
        genres.Should().HaveCountGreaterThanOrEqualTo(2);
    }
    
    [Fact]
    public async Task GetGenreById_ReturnsCorrectGenre()
    {
        var allGenresResponse = await _client.GetAsync("/genres");
        allGenresResponse.EnsureSuccessStatusCode();

        var genres = await allGenresResponse.Content.ReadFromJsonAsync<List<GenreResponse>>();
        var drama = genres!.First(g => g.Title == "Drama");
        
        var singleGenreResponse = await _client.GetAsync($"/genres/{drama.Id}");
        singleGenreResponse.EnsureSuccessStatusCode();

        var genre = await singleGenreResponse.Content.ReadFromJsonAsync<GenreResponse>();
        genre.Should().NotBeNull();
        genre!.Id.Should().Be(drama.Id);
        genre.Title.Should().Be("Drama");
    }
}