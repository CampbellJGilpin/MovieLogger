using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.services;
using NSubstitute;
using Xunit;

namespace movielogger.services.tests.services;

public class MoviesServiceTests : BaseServiceTest
{
    private readonly MoviesService _service;

    public MoviesServiceTests()
    {
        _service = new MoviesService(DbContext, Mapper);
    }
    
    [Fact]
    public async Task GetAllMoviesAsync_ShouldReturnOnlyNonDeletedMovies()
    {
        // Arrange
        var movies = new List<Movie>
        {
            new() { Id = 1, Title = "Interstellar", Genre = new Genre { Id = 1, Title = "Sci-Fi" }, IsDeleted = false },
            new() { Id = 2, Title = "Inception", Genre = new Genre { Id = 2, Title = "Thriller" }, IsDeleted = false },
            new() { Id = 3, Title = "Old Deleted Movie", Genre = new Genre { Id = 3, Title = "Drama" }, IsDeleted = true }
        };

        DbContext.Movies.AddRange(movies);
        await DbContext.SaveChangesAsync();

        var expectedDtos = new List<MovieDto>
        {
            new() { Id = 1, Title = "Interstellar", GenreId = 1, Genre = new GenreDto { Id = 1, Title = "Sci-Fi" } },
            new() { Id = 2, Title = "Inception", GenreId = 2, Genre = new GenreDto { Id = 2, Title = "Thriller" } }
        };

        Mapper.Map<List<MovieDto>>(Arg.Is<List<Movie>>(m => m.Count == 2)).Returns(expectedDtos);

        // Act
        var result = await _service.GetAllMoviesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.DoesNotContain(result, m => m.Title == "Old Deleted Movie");
        Assert.Contains(result, m => m.Title == "Interstellar");
        Assert.Contains(result, m => m.Title == "Inception");
    }
}