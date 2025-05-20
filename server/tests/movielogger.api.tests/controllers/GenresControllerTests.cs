using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.genres;
using movielogger.api.models.responses.genres;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class GenresControllerTests : BaseTestController
{
    [Fact]
    public async Task Get_Always_ReturnsAllGenres()
    {
        // Arrange 
        const string firstGenreTitle = "Genre 1";
        const int firstGenreId = 1;
        const string secondGenreTitle = "Genre 2";
        const int secondGenreId = 2;
        
        
        var mockGenres = new GenreDto[]
        {
            new() { Id = firstGenreId, Title = firstGenreTitle },
            new() { Id = secondGenreId, Title = secondGenreTitle },
        }.ToList();

        _factory.GenresServiceMock.GetGenresAsync().Returns(mockGenres);
        
        // Act
        var response = await _client.GetAsync("/Genres");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<List<GenreResponse>>();
        
        Assert.NotNull(content);
        Assert.Equal(2, content.Count);
        Assert.Equal(firstGenreTitle, content[0].Title);
        Assert.Equal(firstGenreId, content[0].Id);
        Assert.Equal(secondGenreTitle, content[1].Title);
        Assert.Equal(secondGenreId, content[1].Id);
    }
    
    [Fact]
    public async Task Get_IfExists_ReturnsGenre()
    {
        // Arrange 
        const string genreTitle = "Genre 3";
        const int genreId = 3;
        
        var mockGenre = new GenreDto { Id = genreId, Title = genreTitle };
        
        _factory.GenresServiceMock.GetGenreByIdAsync(genreId).Returns(mockGenre);
        
        // Act
        var response = await _client.GetAsync($"/Genres/{genreId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<GenreResponse>();

        Assert.NotNull(content);
        Assert.Equal(genreTitle, content.Title);
        Assert.Equal(genreId, content.Id);
    }
    
    [Fact]
    public async Task Post_WithValidData_SavesGenre()
    {
        // Arrange 
        const string genreTitle = "Genre 4";
        const int genreId = 4;
        
        var newGenre = new CreateGenreRequest { Title = genreTitle };
        var mockGenre = new GenreDto { Id = genreId, Title = genreTitle };
        
        _factory.GenresServiceMock.CreateGenreAsync(Arg.Any<GenreDto>()).Returns(mockGenre);
        
        // Act
        var response = await _client.PostAsync("/Genres", JsonContent.Create(newGenre));
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<GenreResponse>();
        
        Assert.NotNull(content);
        Assert.Equal(genreId, content.Id);
        Assert.Equal(genreTitle, content.Title);
    }
}