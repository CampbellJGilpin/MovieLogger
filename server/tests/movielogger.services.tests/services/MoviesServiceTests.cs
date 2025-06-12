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
using AutoFixture;
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
        // Arrange
        var genre = Fixture.Create<Genre>();

        var movies = Fixture.Build<Movie>()
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .CreateMany(2)
            .AsQueryable();

        var mockSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act 
        var result = (await _service.GetAllMoviesAsync());

        // Assert
        result.Movies.Should().HaveCount(2);
        result.Movies.All(m => m.Genre.Title == genre.Title).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllMoviesAsync_RequestTenMovies_ReturnsTenNonDeletedMovies()
    {
        // Arrange
        const int pageNumber = 1;
        const int movieCount = 10;
        
        var genre = Fixture.Create<Genre>();

        var deletedMovies = Fixture.Build<Movie>()
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, true)
            .CreateMany(12).ToList();

        var activeMovies = Fixture.Build<Movie>()
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .CreateMany(12).ToList();

        var allMovies = new List<Movie>();
        allMovies.AddRange(deletedMovies);
        allMovies.AddRange(activeMovies);
        
        var movieEntities = allMovies.AsQueryable();
        var mockSet = movieEntities.BuildMockDbSet();

        _dbContext.Movies.Returns(mockSet);

        // Act 
        var result = (await _service.GetAllMoviesAsync(pageNumber, movieCount));

        // Assert
        result.Movies.Should().HaveCount(movieCount);
        result.TotalCount.Should().Be(activeMovies.Count);
        result.Movies.All(m => m.IsDeleted == false).Should().BeTrue();
    }

    [Fact]
    public async Task CreateMovieAsync_ValidDto_AddsMovieAndReturnsMappedDto()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();

        var inputDto = Fixture.Build<MovieDto>()
            .With(m => m.Genre, new GenreDto { Id = genre.Id, Title = genre.Title })
            .Create();

        var returnedMovieFromDb = Fixture.Build<Movie>()
            .With(m => m.Id, 1)
            .With(m => m.Title, inputDto.Title)
            .With(m => m.Description, inputDto.Description)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.Genre, genre)
            .Create();

        var mockSet = new List<Movie> { returnedMovieFromDb }
            .AsQueryable()
            .BuildMockDbSet();

        mockSet.Add(Arg.Do<Movie>(m =>
        {
            m.Id = 1;
            m.Genre = genre;
        }));

        _dbContext.Movies.Returns(mockSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act 
        var result = await _service.CreateMovieAsync(inputDto);

        // Assert
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        mockSet.Received(1).Add(Arg.Any<Movie>());

        result.Should().NotBeNull();
        result.Title.Should().Be(inputDto.Title);
        result.Description.Should().Be(inputDto.Description);
        result.Genre.Should().NotBeNull();
        result.Genre.Title.Should().Be(genre.Title);
        result.Genre.Id.Should().Be(genre.Id);
    }
}