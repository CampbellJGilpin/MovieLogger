using AutoFixture;
using AutoMapper;
using EntityFrameworkCore.Testing.NSubstitute;
using FluentAssertions;
using MockQueryable.NSubstitute;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;
using movielogger.services.services;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;

namespace movielogger.services.tests.services;

public class LibraryServiceTests : BaseServiceTest
{
    private readonly ILibraryService _service;
    private const int UserId = 42;

    public LibraryServiceTests()
    {
        var logger = Substitute.For<ILogger<LibraryService>>();
        _service = new LibraryService(_dbContext, _mapper, logger);
    }

    [Fact]
    public async Task GetLibraryByUserIdAsync_ValidId_ReturnsMappedLibrary()
    {
        // Arrange
        var movie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();
        var userMovie = Fixture.Build<UserMovie>()
            .With(um => um.UserId, UserId)
            .With(um => um.Movie, movie)
            .With(um => um.MovieId, movie.Id)
            .Create();

        var userMovies = new List<UserMovie> { userMovie }.AsQueryable();
        var users = new List<User> { new User { Id = UserId, IsDeleted = false } }.AsQueryable();

        var mockUserMoviesSet = userMovies.BuildMockDbSet();
        var mockUsersSet = users.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMoviesSet);
        _dbContext.Users.Returns(mockUsersSet);

        // Act
        var result = await _service.GetLibraryByUserIdAsync(UserId);

        // Assert
        result.UserId.Should().Be(UserId);
        result.LibraryItems.Should().HaveCount(1);
        result.LibraryItems.First().MovieId.Should().Be(movie.Id);
    }

    [Fact]
    public async Task GetLibraryFavouritesByUserIdAsync_ValidId_ReturnsMappedLibrary()
    {
        // Arrange
        var movie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();
        var userMovie = Fixture.Build<UserMovie>().With(um => um.UserId, UserId)
            .With(um => um.Favourite, true)
            .With(um => um.Movie, movie)
            .With(um => um.MovieId, movie.Id)
            .Create();

        var userMovies = new List<UserMovie> { userMovie }.AsQueryable();
        var users = new List<User> { new User { Id = UserId, IsDeleted = false } }.AsQueryable();

        var mockUserMoviesSet = userMovies.BuildMockDbSet();
        var mockUsersSet = users.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMoviesSet);
        _dbContext.Users.Returns(mockUsersSet);

        // Act
        var result = await _service.GetLibraryFavouritesByUserIdAsync(UserId);

        // Assert
        result.UserId.Should().Be(UserId);
        result.LibraryItems.Should().ContainSingle(i => i.MovieId == movie.Id);
    }

    [Fact]
    public async Task GetLibraryFavouritesByUserIdAsync_IncludeNonFavourites_FiltersOutNonFavouriteMovies()
    {
        // Arrange
        var favouriteMovie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();
        var notFavouriteMovie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();

        var favUserMovie = Fixture.Build<UserMovie>()
            .With(um => um.UserId, UserId)
            .With(um => um.Favourite, true)
            .With(um => um.MovieId, favouriteMovie.Id)
            .With(um => um.Movie, favouriteMovie)
            .Create();

        var nonFavUserMovie = Fixture.Build<UserMovie>()
            .With(um => um.UserId, UserId)
            .With(um => um.Favourite, false)
            .With(um => um.MovieId, notFavouriteMovie.Id)
            .With(um => um.Movie, notFavouriteMovie)
            .Create();

        var userMovies = new List<UserMovie> { favUserMovie, nonFavUserMovie }.AsQueryable();
        var users = new List<User> { new User { Id = UserId, IsDeleted = false } }.AsQueryable();

        var mockUserMoviesSet = userMovies.BuildMockDbSet();
        var mockUsersSet = users.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMoviesSet);
        _dbContext.Users.Returns(mockUsersSet);

        // Act
        var result = await _service.GetLibraryFavouritesByUserIdAsync(UserId);

        // Assert
        result.LibraryItems.Should().HaveCount(1);
        result.LibraryItems.Should().ContainSingle(i => i.MovieId == favouriteMovie.Id);
        result.LibraryItems.Should().NotContain(i => i.MovieId == notFavouriteMovie.Id);
    }

    [Fact]
    public async Task GetLibraryWatchlistByUserIdAsync_ValidId_ReturnsMappedLibrary()
    {
        // Arrange
        var movie = Fixture.Build<Movie>()
            .With(m => m.IsDeleted, false)
            .Create();

        var userMovie = Fixture.Build<UserMovie>().With(um => um.UserId, UserId)
            .With(um => um.UpcomingViewDate, DateTime.Today.AddDays(1))
            .With(um => um.Movie, movie)
            .With(um => um.MovieId, movie.Id)
            .Create();

        var userMovies = new List<UserMovie> { userMovie }.AsQueryable();
        var users = new List<User> { new User { Id = UserId, IsDeleted = false } }.AsQueryable();

        var mockUserMoviesSet = userMovies.BuildMockDbSet();
        var mockUsersSet = users.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockUserMoviesSet);
        _dbContext.Users.Returns(mockUsersSet);

        // Act
        var result = await _service.GetLibraryWatchlistByUserIdAsync(UserId);

        // Assert
        result.UserId.Should().Be(UserId);
        result.LibraryItems.Should().ContainSingle(i => i.MovieId == movie.Id);
    }

    [Fact]
    public async Task UpdateLibraryEntryAsync_NewEntry_CreatesAndReturnsDto()
    {
        // Arrange
        var user = Fixture.Build<User>().With(m => m.Id, UserId).Create();
        var movie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();
        var dto = Fixture.Build<LibraryItemDto>().With(d => d.MovieId, movie.Id).Create();
        var userMovie = Fixture.Build<UserMovie>()
            .With(um => um.UserId, UserId)
            .With(um => um.MovieId, movie.Id)
            .Create();

        // Setup empty UserMovies collection
        var existing = new List<UserMovie>().AsQueryable().BuildMockDbSet();
        _dbContext.UserMovies.Returns(existing);

        // Setup Movies collection to return our movie
        var movies = new List<Movie> { movie }.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(movies);

        // Setup Movies collection to return our movie
        var users = new List<User> { user }.AsQueryable().BuildMockDbSet();
        _dbContext.Users.Returns(users);

        // Setup Movies collection to return our movie
        var userMovies = new List<UserMovie> { userMovie }.AsQueryable().BuildMockDbSet();
        _dbContext.UserMovies.Returns(userMovies);

        var added = new List<UserMovie>();
        _dbContext.UserMovies.When(x => x.Add(Arg.Any<UserMovie>())).Do(x => added.Add(x.Arg<UserMovie>()));
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _service.UpdateLibraryEntryAsync(UserId, dto);

        // Assert
        result.MovieId.Should().Be(dto.MovieId);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateLibraryEntryAsync_ExistingEntry_UpdatesAndReturnsDto()
    {
        // Arrange
        var user = Fixture.Build<User>().With(m => m.Id, UserId).Create();
        var movie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();
        var userMovie = Fixture.Build<UserMovie>()
            .With(um => um.UserId, UserId)
            .With(um => um.MovieId, movie.Id)
            .With(um => um.Favourite, false)
            .With(um => um.Movie, movie)
            .Create();

        // Setup UserMovies collection with existing entry
        var mockSet = new List<UserMovie> { userMovie }.AsQueryable().BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockSet);

        // Setup Movies collection
        var movies = new List<Movie> { movie }.AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(movies);

        // Setup Movies collection to return our movie
        var users = new List<User> { user }.AsQueryable().BuildMockDbSet();
        _dbContext.Users.Returns(users);

        var dto = _mapper.Map<LibraryItemDto>(userMovie);
        dto.Favourite = true;

        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _service.UpdateLibraryEntryAsync(UserId, dto);

        // Assert
        result.Favourite.Should().BeTrue();
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateLibraryEntryAsync_NonExistentUser_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = Fixture.Build<LibraryItemDto>()
            .With(d => d.MovieId, 999)
            .Create();

        // Setup empty Movies collection
        var movies = new List<Movie>().AsQueryable().BuildMockDbSet();
        _dbContext.Movies.Returns(movies);

        // Act
        var act = () => _service.UpdateLibraryEntryAsync(UserId, dto);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetLibraryItemAsync_ExistingEntry_ReturnsDto()
    {
        // Arrange
        var movie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();
        var userMovie = Fixture.Build<UserMovie>()
            .With(um => um.UserId, UserId)
            .With(um => um.MovieId, movie.Id)
            .With(um => um.Movie, movie)
            .Create();

        var mockSet = new List<UserMovie> { userMovie }.AsQueryable().BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockSet);

        // Act
        var result = await _service.GetLibraryItemAsync(UserId, movie.Id);

        // Assert
        result.Should().NotBeNull();
        result!.MovieId.Should().Be(movie.Id);
    }

    [Fact]
    public async Task GetLibraryItemAsync_NonExistentEntry_ReturnsNull()
    {
        // Arrange
        var mockSet = new List<UserMovie>().AsQueryable().BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockSet);

        // Act
        var result = await _service.GetLibraryItemAsync(UserId, 999);

        // Assert
        result.Should().BeNull();
    }
}
