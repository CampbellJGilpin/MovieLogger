using AutoMapper;
using EntityFrameworkCore.Testing.NSubstitute;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;
using movielogger.services.services;
using movielogger.services.tests.services;
using NSubstitute;
using Xunit;
using System.Linq.Expressions;
using MockQueryable.NSubstitute;

namespace movielogger.services.tests.services;

public class MoviesServiceTests : BaseServiceTest
{
    private readonly IMoviesService _service;

    public MoviesServiceTests()
    {
        _service = new MoviesService(_dbContext, _mapper);
    }

    [Fact]
    public async Task GetAllMoviesAsync_ReturnsMappedMovies()
    {
        var genre = new Genre { Id = 1, Title = "Action" };
        var movieEntities = new List<Movie>
        {
            new() { Id = 1, Title = "Movie 1", Genre = genre, IsDeleted = false },
            new() { Id = 2, Title = "Movie 2", Genre = genre, IsDeleted = false }
        }.AsQueryable();

        var mockSet = movieEntities.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        var result = (await _service.GetAllMoviesAsync()).ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(m => m.Title == "Movie 1");
        result.Should().Contain(m => m.Title == "Movie 2");
    }

    [Fact]
    public async Task GetAllMoviesAsync_ReturnsNonDeletedMovies()
    {
        var genre = new Genre { Id = 1, Title = "Action" };
        var movieEntities = new List<Movie>
        {
            new() { Id = 1, Title = "Movie 1", Genre = genre, IsDeleted = true },
            new() { Id = 2, Title = "Movie 2", Genre = genre, IsDeleted = false }
        }.AsQueryable();

        var mockSet = movieEntities.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        var result = (await _service.GetAllMoviesAsync()).ToList();

        result.Should().HaveCount(1);
        result.Should().ContainSingle(m => m.Title == "Movie 2");
        result.Should().NotContain(m => m.Title == "Movie 1");
    }

    [Fact]
    public async Task CreateMovieAsync_ValidDto_AddsMovieAndReturnsMappedDto()
    {
        var genre = new Genre { Id = 1, Title = "Action" };

        var inputDto = new MovieDto
        {
            Title = "New Movie",
            Description = "About film",
            Genre = new GenreDto { Id = 1, Title = "Action" }
        };

        var returnedMovieFromDb = new Movie
        {
            Id = 1,
            Title = "New Movie",
            Description = "About film",
            GenreId = 1,
            Genre = genre
        };

        var movieQueryable = new List<Movie> { returnedMovieFromDb }.AsQueryable();
        var mockSet = movieQueryable.BuildMockDbSet();

        // Simulate EF behavior on Add
        mockSet.Add(Arg.Do<Movie>(m =>
        {
            m.Id = 1;
            m.Genre = genre;
        }));

        _dbContext.Movies.Returns(mockSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _service.CreateMovieAsync(inputDto);

        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        result.Should().NotBeNull();
        result.Title.Should().Be("New Movie");
        result.Description.Should().Be("About film");
        result.Genre.Should().NotBeNull();
        result.Genre.Title.Should().Be("Action");
        result.Genre.Id.Should().Be(1);
    }
}