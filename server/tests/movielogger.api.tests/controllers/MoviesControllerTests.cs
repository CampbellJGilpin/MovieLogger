using System.Net.Http.Json;
using movielogger.api.models.responses.movies;
using movielogger.api.tests.fixtures;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class MoviesControllerTests : BaseTestController
{
    [Fact]
    public async Task GetAllMovies_ReturnsSuccess()
    {
        // Arrange 
        var mockMovies = new MovieDto[]
        {
            new()
            {
                Id = 1, 
                Title = "The Matrix", 
                ReleaseDate = DateTime.Now,
                Genre = new GenreDto{ Id = 1, Title = "Action" },
            },
            new()
            {
                Id = 1, 
                Title = "Princess Mononoke", 
                ReleaseDate = DateTime.Now,
                Genre = new GenreDto { Id = 2, Title = "Adventure" }
            }
        }.ToList();

        _factory.MoviesServiceMock.GetAllMoviesAsync().Returns(mockMovies);
        
        // Act
        var response = await _client.GetAsync("/Movies");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<List<MovieResponse>>();
        
        Assert.NotNull(content);
        Assert.Equal(2, content.Count);
        Assert.Equal("The Matrix", content[0].Title);
    }

    [Fact]
    public async Task GetMovieById_ReturnsSuccess()
    {
        // Arrange 
        var mockMovie = new MovieDto
        {
            Id = 3,
            Title = "The Thing",
            ReleaseDate = DateTime.Now,
            Genre = new GenreDto { Id = 3, Title = "Horror" }
        };
        
        _factory.MoviesServiceMock.GetMovieByIdAsync(3).Returns(mockMovie);
        
        // Act
        var response = await _client.GetAsync("/Movies/3");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<MovieResponse>();

        Assert.NotNull(content);
        Assert.Equal("The Thing", content.Title);
        Assert.Equal(3, content.Id);
        Assert.Equal("Horror", content.Genre.Title);
    }
}