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
        var mockGenres = new GenreDto[]
        {
            new() { Id = 1, Title = "Action" },
            new() { Id = 2, Title = "Adventure" },
        }.ToList();

        _factory.GenresServiceMock.GetGenresAsync().Returns(mockGenres);
        
        // Act
        var response = await _client.GetAsync("/Genres");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<List<GenreResponse>>();
        
        Assert.NotNull(content);
        Assert.Equal(2, content.Count);
        Assert.Equal("Action", content[0].Title);
    }
    
    [Fact]
    public async Task Get_IfExists_ReturnsGenre()
    {
        // Arrange 
        var movieGenre = new GenreDto()
        {
            Id = 4,
            Title = "Horror",
        };
        
        _factory.GenresServiceMock.GetGenreByIdAsync(4).Returns(movieGenre);
        
        // Act
        var response = await _client.GetAsync("/Genres/4");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<GenreResponse>();

        Assert.NotNull(content);
        Assert.Equal("Horror", content.Title);
        Assert.Equal(4, content.Id);
    }
    
    [Fact]
    public async Task Post_WithValidData_SavesGenre()
    {
        // Arrange 
        var newGenre = new CreateGenreRequest { Title = "Romance" };
        var mockGenre = new GenreDto { Id = 21, Title = "Romance" };
        
        _factory.GenresServiceMock.CreateGenreAsync(Arg.Any<GenreDto>()).Returns(mockGenre);
        
        // Act
        var response = await _client.PostAsync("/Genres", JsonContent.Create(newGenre));
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<GenreResponse>();
        
        Assert.NotNull(content);
        Assert.Equal(21, content.Id);
        Assert.Equal("Romance", content.Title);
    }
}