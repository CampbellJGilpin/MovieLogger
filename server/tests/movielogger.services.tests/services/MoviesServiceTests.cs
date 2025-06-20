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

    [Fact]
    public async Task GetAllMoviesForUserAsync_UserHasNoMovies_AllFlagsFalse()
    {
        // Arrange
        var userId = 1;
        var genre = Fixture.Create<Genre>();
        var movies = Fixture.Build<Movie>()
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .CreateMany(3)
            .AsQueryable();
        var mockMovieSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockMovieSet);

        var userMovies = new List<UserMovie>().AsQueryable();
        var mockUserMovieSet = userMovies.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMovieSet);

        // Act
        var (result, totalCount, totalPages) = await _service.GetAllMoviesForUserAsync(userId);

        // Assert
        result.Should().HaveCount(3);
        result.All(m => m.OwnsMovie == false && m.IsFavourite == false).Should().BeTrue();
        totalCount.Should().Be(3);
        totalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetAllMoviesForUserAsync_UserHasOwnedAndFavoritedMovies_FlagsAreCorrect()
    {
        // Arrange
        var userId = 2;
        var genre = Fixture.Create<Genre>();
        var movies = Fixture.Build<Movie>()
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .CreateMany(3)
            .ToList();
        var mockMovieSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockMovieSet);

        var userMovies = new List<UserMovie>
        {
            new UserMovie { UserId = userId, MovieId = movies[0].Id, OwnsMovie = true, Favourite = false },
            new UserMovie { UserId = userId, MovieId = movies[1].Id, OwnsMovie = false, Favourite = true },
            // movies[2] is neither owned nor favorited
        }.AsQueryable();
        var mockUserMovieSet = userMovies.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMovieSet);

        // Act
        var (result, totalCount, totalPages) = await _service.GetAllMoviesForUserAsync(userId);

        // Assert
        result.Should().HaveCount(3);
        result.Single(m => m.Id == movies[0].Id).OwnsMovie.Should().BeTrue();
        result.Single(m => m.Id == movies[0].Id).IsFavourite.Should().BeFalse();
        result.Single(m => m.Id == movies[1].Id).OwnsMovie.Should().BeFalse();
        result.Single(m => m.Id == movies[1].Id).IsFavourite.Should().BeTrue();
        result.Single(m => m.Id == movies[2].Id).OwnsMovie.Should().BeFalse();
        result.Single(m => m.Id == movies[2].Id).IsFavourite.Should().BeFalse();
        totalCount.Should().Be(3);
        totalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetAllMoviesForUserAsync_DeletedMoviesAreNotReturned()
    {
        // Arrange
        var userId = 3;
        var genre = Fixture.Create<Genre>();
        var deletedMovie = Fixture.Build<Movie>()
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, true)
            .Create();
        var activeMovie = Fixture.Build<Movie>()
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .Create();
        var movies = new List<Movie> { deletedMovie, activeMovie };
        var mockMovieSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockMovieSet);

        var userMovies = new List<UserMovie>().AsQueryable();
        var mockUserMovieSet = userMovies.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMovieSet);

        // Act
        var (result, totalCount, totalPages) = await _service.GetAllMoviesForUserAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(activeMovie.Id);
        totalCount.Should().Be(1);
        totalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetAllMoviesForUserAsync_SearchFiltersResults()
    {
        // Arrange
        var userId = 4;
        var genre = Fixture.Create<Genre>();
        var movies = new List<Movie>
        {
            new Movie { Id = 1, Title = "The Matrix", Genre = genre, GenreId = genre.Id, IsDeleted = false },
            new Movie { Id = 2, Title = "Inception", Genre = genre, GenreId = genre.Id, IsDeleted = false },
            new Movie { Id = 3, Title = "Interstellar", Genre = genre, GenreId = genre.Id, IsDeleted = false }
        }.AsQueryable();
        var mockMovieSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockMovieSet);

        var userMovies = new List<UserMovie>().AsQueryable();
        var mockUserMovieSet = userMovies.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMovieSet);

        // Act
        var (result, totalCount, totalPages) = await _service.GetAllMoviesForUserAsync(userId, "inc");

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Inception");
        totalCount.Should().Be(1);
        totalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetAllMoviesForUserAsync_PaginationWorks()
    {
        // Arrange
        var userId = 5;
        var genre = Fixture.Create<Genre>();
        var movies = Fixture.Build<Movie>()
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .CreateMany(25)
            .ToList();
        var mockMovieSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockMovieSet);

        var userMovies = new List<UserMovie>().AsQueryable();
        var mockUserMovieSet = userMovies.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMovieSet);

        // Act
        var (result, totalCount, totalPages) = await _service.GetAllMoviesForUserAsync(userId, null, 2, 10);

        // Assert
        result.Should().HaveCount(10);
        totalCount.Should().Be(25);
        totalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetMovieByIdAsync_ValidId_ReturnsMovie()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var movie = Fixture.Build<Movie>()
            .With(m => m.Id, 1)
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .Create();

        var movies = new List<Movie> { movie }.AsQueryable();
        var mockSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.GetMovieByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be(movie.Title);
        result.Genre.Should().NotBeNull();
        result.Genre.Title.Should().Be(genre.Title);
    }

    [Fact]
    public async Task GetMovieByIdAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var movies = new List<Movie>().AsQueryable();
        var mockSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var act = () => _service.GetMovieByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Movie with ID 999 not found.");
    }

    [Fact]
    public async Task GetMovieByIdAsync_DeletedMovie_ThrowsKeyNotFoundException()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var movie = Fixture.Build<Movie>()
            .With(m => m.Id, 1)
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, true)
            .Create();

        var movies = new List<Movie> { movie }.AsQueryable();
        var mockSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var act = () => _service.GetMovieByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Movie with ID 1 not found.");
    }

    [Fact]
    public async Task UpdateMovieAsync_ValidMovie_UpdatesAndReturnsMovie()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var existingMovie = Fixture.Build<Movie>()
            .With(m => m.Id, 1)
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .Create();

        var updateDto = Fixture.Build<MovieDto>()
            .With(m => m.Id, 1)
            .With(m => m.Title, "Updated Title")
            .With(m => m.Description, "Updated Description")
            .Create();

        var movies = new List<Movie> { existingMovie };
        var mockSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.UpdateMovieAsync(1, updateDto);

        // Assert
        await _dbContext.Received(1).SaveChangesAsync();
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task UpdateMovieAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = Fixture.Create<MovieDto>();
        var movies = new List<Movie>().AsQueryable();
        var mockSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var act = () => _service.UpdateMovieAsync(999, updateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Movie with ID 999 not found.");
    }

    [Fact]
    public async Task UpdateMovieAsync_DeletedMovie_ThrowsKeyNotFoundException()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var deletedMovie = Fixture.Build<Movie>()
            .With(m => m.Id, 1)
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, true)
            .Create();

        var updateDto = Fixture.Create<MovieDto>();
        var movies = new List<Movie> { deletedMovie }.AsQueryable();
        var mockSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var act = () => _service.UpdateMovieAsync(1, updateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Movie with ID 1 not found.");
    }

    [Fact]
    public async Task DeleteMovieAsync_ValidMovie_ReturnsTrueAndMarksAsDeleted()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var movie = Fixture.Build<Movie>()
            .With(m => m.Id, 1)
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, false)
            .Create();

        var movies = new List<Movie> { movie };
        var mockSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.DeleteMovieAsync(1);

        // Assert
        await _dbContext.Received(1).SaveChangesAsync();
        result.Should().BeTrue();
        movie.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteMovieAsync_InvalidId_ReturnsFalse()
    {
        // Arrange
        var movies = new List<Movie>().AsQueryable();
        var mockSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.DeleteMovieAsync(999);

        // Assert
        result.Should().BeFalse();
        await _dbContext.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteMovieAsync_AlreadyDeletedMovie_ReturnsFalse()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var deletedMovie = Fixture.Build<Movie>()
            .With(m => m.Id, 1)
            .With(m => m.Genre, genre)
            .With(m => m.GenreId, genre.Id)
            .With(m => m.IsDeleted, true)
            .Create();

        var movies = new List<Movie> { deletedMovie }.AsQueryable();
        var mockSet = movies.BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.DeleteMovieAsync(1);

        // Assert
        result.Should().BeFalse();
        await _dbContext.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task SearchMoviesAsync_ValidQuery_ReturnsMatchingMovies()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var movies = new List<Movie>
        {
            Fixture.Build<Movie>()
                .With(m => m.Id, 1)
                .With(m => m.Title, "The Matrix")
                .With(m => m.Genre, genre)
                .With(m => m.GenreId, genre.Id)
                .With(m => m.IsDeleted, false)
                .Create(),
            Fixture.Build<Movie>()
                .With(m => m.Id, 2)
                .With(m => m.Title, "Matrix Reloaded")
                .With(m => m.Genre, genre)
                .With(m => m.GenreId, genre.Id)
                .With(m => m.IsDeleted, false)
                .Create(),
            Fixture.Build<Movie>()
                .With(m => m.Id, 3)
                .With(m => m.Title, "Star Wars")
                .With(m => m.Genre, genre)
                .With(m => m.GenreId, genre.Id)
                .With(m => m.IsDeleted, false)
                .Create()
        };

        var mockSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.SearchMoviesAsync("matrix");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.Title == "The Matrix");
        result.Should().Contain(m => m.Title == "Matrix Reloaded");
        result.Should().NotContain(m => m.Title == "Star Wars");
    }

    [Fact]
    public async Task SearchMoviesAsync_QueryWithDifferentCase_ReturnsMatchingMovies()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var movies = new List<Movie>
        {
            Fixture.Build<Movie>()
                .With(m => m.Id, 1)
                .With(m => m.Title, "The Matrix")
                .With(m => m.Genre, genre)
                .With(m => m.GenreId, genre.Id)
                .With(m => m.IsDeleted, false)
                .Create()
        };

        var mockSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.SearchMoviesAsync("MATRIX");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(m => m.Title == "The Matrix");
    }

    [Fact]
    public async Task SearchMoviesAsync_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var movies = new List<Movie>
        {
            Fixture.Build<Movie>()
                .With(m => m.Id, 1)
                .With(m => m.Title, "The Matrix")
                .With(m => m.Genre, genre)
                .With(m => m.GenreId, genre.Id)
                .With(m => m.IsDeleted, false)
                .Create()
        };

        var mockSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.SearchMoviesAsync("nonexistent");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchMoviesAsync_DeletedMoviesAreExcluded()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        var movies = new List<Movie>
        {
            Fixture.Build<Movie>()
                .With(m => m.Id, 1)
                .With(m => m.Title, "The Matrix")
                .With(m => m.Genre, genre)
                .With(m => m.GenreId, genre.Id)
                .With(m => m.IsDeleted, false)
                .Create(),
            Fixture.Build<Movie>()
                .With(m => m.Id, 2)
                .With(m => m.Title, "Matrix Reloaded")
                .With(m => m.Genre, genre)
                .With(m => m.GenreId, genre.Id)
                .With(m => m.IsDeleted, true)
                .Create()
        };

        var mockSet = movies.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(mockSet);

        // Act
        var result = await _service.SearchMoviesAsync("matrix");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(m => m.Title == "The Matrix");
        result.Should().NotContain(m => m.Title == "Matrix Reloaded");
    }
}