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

public class GenrePreferencesServiceTests : BaseServiceTest
{
    private readonly IGenrePreferencesService _service;

    public GenrePreferencesServiceTests()
    {
        _service = new GenrePreferencesService(_dbContext);
    }

    [Fact]
    public async Task GetUserGenrePreferencesAsync_UserExistsWithViewings_ReturnsCorrectSummary()
    {
        // Arrange
        var userId = 1;
        var user = Fixture.Build<User>().With(u => u.Id, userId).With(u => u.IsDeleted, false).Create();
        var genre = Fixture.Build<Genre>().With(g => g.Id, 1).With(g => g.Title, "Action").Create();
        var movie = Fixture.Build<Movie>().With(m => m.Id, 1).With(m => m.GenreId, genre.Id).With(m => m.Genre, genre).Create();
        var userMovie = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie.Id).With(um => um.Movie, movie).Create();
        var viewing = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie.Id).With(v => v.UserMovie, userMovie).Create();
        var review = Fixture.Build<Review>().With(r => r.Score, 8).Create();
        viewing.Review = review;

        var users = new List<User> { user }.AsQueryable();
        var viewings = new List<Viewing> { viewing }.AsQueryable();

        var mockUsersSet = users.BuildMockDbSet();
        var mockViewingsSet = viewings.BuildMockDbSet();

        _dbContext.Users.Returns(mockUsersSet);
        _dbContext.Viewings.Returns(mockViewingsSet);

        // Act
        var result = await _service.GetUserGenrePreferencesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.TotalMoviesWatched.Should().Be(1);
        result.TotalUniqueGenres.Should().Be(1);
        result.GenrePreferences.Should().HaveCount(1);
        result.GenrePreferences.First().GenreTitle.Should().Be("Action");
        result.GenrePreferences.First().WatchCount.Should().Be(1);
        result.GenrePreferences.First().AverageRating.Should().Be(8.0);
        result.MostWatchedGenre.Should().NotBeNull();
        result.MostWatchedGenre!.GenreTitle.Should().Be("Action");
    }

    [Fact]
    public async Task GetUserGenrePreferencesAsync_UserDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = 999;
        var users = new List<User>().AsQueryable();
        var mockUsersSet = users.BuildMockDbSet();
        _dbContext.Users.Returns(mockUsersSet);

        // Act
        var act = async () => await _service.GetUserGenrePreferencesAsync(userId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with ID {userId} not found.");
    }

    [Fact]
    public async Task GetUserGenrePreferencesAsync_UserExistsButNoViewings_ReturnsEmptySummary()
    {
        // Arrange
        var userId = 1;
        var user = Fixture.Build<User>().With(u => u.Id, userId).With(u => u.IsDeleted, false).Create();
        var users = new List<User> { user }.AsQueryable();
        var viewings = new List<Viewing>().AsQueryable();

        var mockUsersSet = users.BuildMockDbSet();
        var mockViewingsSet = viewings.BuildMockDbSet();

        _dbContext.Users.Returns(mockUsersSet);
        _dbContext.Viewings.Returns(mockViewingsSet);

        // Act
        var result = await _service.GetUserGenrePreferencesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.TotalMoviesWatched.Should().Be(0);
        result.TotalUniqueGenres.Should().Be(0);
        result.GenrePreferences.Should().BeEmpty();
        result.MostWatchedGenre.Should().BeNull();
        result.HighestRatedGenre.Should().BeNull();
        result.LeastWatchedGenre.Should().BeNull();
    }

    [Fact]
    public async Task GetUserGenrePreferencesAsync_MultipleGenres_CalculatesCorrectPercentages()
    {
        // Arrange
        var userId = 1;
        var user = Fixture.Build<User>().With(u => u.Id, userId).With(u => u.IsDeleted, false).Create();

        var actionGenre = Fixture.Build<Genre>().With(g => g.Id, 1).With(g => g.Title, "Action").Create();
        var comedyGenre = Fixture.Build<Genre>().With(g => g.Id, 2).With(g => g.Title, "Comedy").Create();

        var actionMovie = Fixture.Build<Movie>().With(m => m.Id, 1).With(m => m.GenreId, actionGenre.Id).With(m => m.Genre, actionGenre).Create();
        var comedyMovie = Fixture.Build<Movie>().With(m => m.Id, 2).With(m => m.GenreId, comedyGenre.Id).With(m => m.Genre, comedyGenre).Create();

        var actionUserMovie = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, actionMovie.Id).With(um => um.Movie, actionMovie).Create();
        var comedyUserMovie = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, comedyMovie.Id).With(um => um.Movie, comedyMovie).Create();

        var actionViewing1 = Fixture.Build<Viewing>().With(v => v.UserMovieId, actionUserMovie.Id).With(v => v.UserMovie, actionUserMovie).Create();
        var actionViewing2 = Fixture.Build<Viewing>().With(v => v.UserMovieId, actionUserMovie.Id).With(v => v.UserMovie, actionUserMovie).Create();
        var comedyViewing = Fixture.Build<Viewing>().With(v => v.UserMovieId, comedyUserMovie.Id).With(v => v.UserMovie, comedyUserMovie).Create();

        var users = new List<User> { user }.AsQueryable();
        var viewings = new List<Viewing> { actionViewing1, actionViewing2, comedyViewing }.AsQueryable();

        var mockUsersSet = users.BuildMockDbSet();
        var mockViewingsSet = viewings.BuildMockDbSet();

        _dbContext.Users.Returns(mockUsersSet);
        _dbContext.Viewings.Returns(mockViewingsSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _service.GetUserGenrePreferencesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.TotalMoviesWatched.Should().Be(3);
        result.TotalUniqueGenres.Should().Be(2);
        result.GenrePreferences.Should().HaveCount(2);

        var actionPreference = result.GenrePreferences.First(g => g.GenreTitle == "Action");
        var comedyPreference = result.GenrePreferences.First(g => g.GenreTitle == "Comedy");

        actionPreference.WatchCount.Should().Be(2);
        actionPreference.PercentageOfTotalWatches.Should().Be(66.7);
        comedyPreference.WatchCount.Should().Be(1);
        comedyPreference.PercentageOfTotalWatches.Should().Be(33.3);
    }

    [Fact]
    public async Task GetTopGenresByWatchCountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var userId = 1;
        var user = Fixture.Build<User>().With(u => u.Id, userId).With(u => u.IsDeleted, false).Create();
        var genre1 = Fixture.Build<Genre>().With(g => g.Id, 1).With(g => g.Title, "Action").Create();
        var genre2 = Fixture.Build<Genre>().With(g => g.Id, 2).With(g => g.Title, "Comedy").Create();
        var genre3 = Fixture.Build<Genre>().With(g => g.Id, 3).With(g => g.Title, "Drama").Create();

        var movie1 = Fixture.Build<Movie>().With(m => m.Id, 1).With(m => m.GenreId, genre1.Id).With(m => m.Genre, genre1).Create();
        var movie2 = Fixture.Build<Movie>().With(m => m.Id, 2).With(m => m.GenreId, genre2.Id).With(m => m.Genre, genre2).Create();
        var movie3 = Fixture.Build<Movie>().With(m => m.Id, 3).With(m => m.GenreId, genre3.Id).With(m => m.Genre, genre3).Create();

        var userMovie1 = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie1.Id).With(um => um.Movie, movie1).Create();
        var userMovie2 = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie2.Id).With(um => um.Movie, movie2).Create();
        var userMovie3 = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie3.Id).With(um => um.Movie, movie3).Create();

        var viewing1 = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie1.Id).With(v => v.UserMovie, userMovie1).Create();
        var viewing2 = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie2.Id).With(v => v.UserMovie, userMovie2).Create();
        var viewing3 = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie3.Id).With(v => v.UserMovie, userMovie3).Create();

        var users = new List<User> { user }.AsQueryable();
        var viewings = new List<Viewing> { viewing1, viewing2, viewing3 }.AsQueryable();

        var mockUsersSet = users.BuildMockDbSet();
        var mockViewingsSet = viewings.BuildMockDbSet();

        _dbContext.Users.Returns(mockUsersSet);
        _dbContext.Viewings.Returns(mockViewingsSet);

        // Act
        var result = await _service.GetTopGenresByWatchCountAsync(userId, 2);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(g => g.WatchCount);
    }

    [Fact]
    public async Task GetTopGenresByRatingAsync_ReturnsCorrectOrder()
    {
        // Arrange
        var userId = 1;
        var user = Fixture.Build<User>().With(u => u.Id, userId).With(u => u.IsDeleted, false).Create();
        var genre1 = Fixture.Build<Genre>().With(g => g.Id, 1).With(g => g.Title, "Action").Create();
        var genre2 = Fixture.Build<Genre>().With(g => g.Id, 2).With(g => g.Title, "Comedy").Create();

        var movie1 = Fixture.Build<Movie>().With(m => m.Id, 1).With(m => m.GenreId, genre1.Id).With(m => m.Genre, genre1).Create();
        var movie2 = Fixture.Build<Movie>().With(m => m.Id, 2).With(m => m.GenreId, genre2.Id).With(m => m.Genre, genre2).Create();

        var userMovie1 = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie1.Id).With(um => um.Movie, movie1).Create();
        var userMovie2 = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie2.Id).With(um => um.Movie, movie2).Create();

        var viewing1 = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie1.Id).With(v => v.UserMovie, userMovie1).Create();
        var viewing2 = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie2.Id).With(v => v.UserMovie, userMovie2).Create();

        var review1 = Fixture.Build<Review>().With(r => r.Score, 9).Create();
        var review2 = Fixture.Build<Review>().With(r => r.Score, 7).Create();
        viewing1.Review = review1;
        viewing2.Review = review2;

        var users = new List<User> { user }.AsQueryable();
        var viewings = new List<Viewing> { viewing1, viewing2 }.AsQueryable();

        var mockUsersSet = users.BuildMockDbSet();
        var mockViewingsSet = viewings.BuildMockDbSet();

        _dbContext.Users.Returns(mockUsersSet);
        _dbContext.Viewings.Returns(mockViewingsSet);

        // Act
        var result = await _service.GetTopGenresByRatingAsync(userId, 2);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(g => g.AverageRating);
        result.First().GenreTitle.Should().Be("Action");
        result.First().AverageRating.Should().Be(9.0);
    }

    [Fact]
    public async Task GetLeastWatchedGenresAsync_ReturnsCorrectOrder()
    {
        // Arrange
        var userId = 1;
        var user = Fixture.Build<User>().With(u => u.Id, userId).With(u => u.IsDeleted, false).Create();
        var genre1 = Fixture.Build<Genre>().With(g => g.Id, 1).With(g => g.Title, "Action").Create();
        var genre2 = Fixture.Build<Genre>().With(g => g.Id, 2).With(g => g.Title, "Comedy").Create();

        var movie1 = Fixture.Build<Movie>().With(m => m.Id, 1).With(m => m.GenreId, genre1.Id).With(m => m.Genre, genre1).Create();
        var movie2 = Fixture.Build<Movie>().With(m => m.Id, 2).With(m => m.GenreId, genre2.Id).With(m => m.Genre, genre2).Create();

        var userMovie1 = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie1.Id).With(um => um.Movie, movie1).Create();
        var userMovie2 = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie2.Id).With(um => um.Movie, movie2).Create();

        var viewing1 = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie1.Id).With(v => v.UserMovie, userMovie1).Create();
        var viewing2 = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie2.Id).With(v => v.UserMovie, userMovie2).Create();
        var viewing3 = Fixture.Build<Viewing>().With(v => v.UserMovieId, userMovie2.Id).With(v => v.UserMovie, userMovie2).Create();

        var users = new List<User> { user }.AsQueryable();
        var viewings = new List<Viewing> { viewing1, viewing2, viewing3 }.AsQueryable();

        var mockUsersSet = users.BuildMockDbSet();
        var mockViewingsSet = viewings.BuildMockDbSet();

        _dbContext.Users.Returns(mockUsersSet);
        _dbContext.Viewings.Returns(mockViewingsSet);

        // Act
        var result = await _service.GetLeastWatchedGenresAsync(userId, 2);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeInAscendingOrder(g => g.WatchCount);
        result.First().GenreTitle.Should().Be("Action");
        result.First().WatchCount.Should().Be(1);
    }

    [Fact]
    public async Task GetGenreWatchTrendsAsync_ReturnsCorrectTrends()
    {
        // Arrange
        var userId = 1;
        var user = Fixture.Build<User>().With(u => u.Id, userId).With(u => u.IsDeleted, false).Create();
        var genre = Fixture.Build<Genre>().With(g => g.Id, 1).With(g => g.Title, "Action").Create();
        var movie = Fixture.Build<Movie>().With(m => m.Id, 1).With(m => m.GenreId, genre.Id).With(m => m.Genre, genre).Create();
        var userMovie = Fixture.Build<UserMovie>().With(um => um.UserId, userId).With(um => um.MovieId, movie.Id).With(um => um.Movie, movie).Create();

        var viewing = Fixture.Build<Viewing>()
            .With(v => v.UserMovieId, userMovie.Id)
            .With(v => v.UserMovie, userMovie)
            .With(v => v.DateViewed, DateTime.UtcNow.AddMonths(-1))
            .Create();

        var users = new List<User> { user }.AsQueryable();
        var viewings = new List<Viewing> { viewing }.AsQueryable();

        var mockUsersSet = users.BuildMockDbSet();
        var mockViewingsSet = viewings.BuildMockDbSet();

        _dbContext.Users.Returns(mockUsersSet);
        _dbContext.Viewings.Returns(mockViewingsSet);

        // Act
        var result = await _service.GetGenreWatchTrendsAsync(userId, 6);

        // Assert
        result.Should().NotBeEmpty();
        // Check that the result contains at least one key with "Action" in it
        result.Keys.Should().Contain(k => k.Contains("Action"));
    }
}