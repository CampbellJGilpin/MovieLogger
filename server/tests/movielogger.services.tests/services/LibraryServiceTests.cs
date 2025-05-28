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

namespace movielogger.services.tests.services;

public class LibraryServiceTests : BaseServiceTest
{
    private readonly ILibraryService _service;
    private const int UserId = 42;

    public LibraryServiceTests()
    {
        _service = new LibraryService(_dbContext, _mapper);
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

        var mockSet = userMovies.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockSet);

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

        var mockSet = userMovies.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockSet);

        // Act
        var result = await _service.GetLibraryFavouritesByUserIdAsync(UserId);

        // Assert
        result.UserId.Should().Be(UserId);
        result.LibraryItems.Should().ContainSingle(i => i.MovieId == movie.Id);
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

        var mockSet = userMovies.BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockSet);

        // Act
        var result = await _service.GetLibraryWatchlistByUserIdAsync(UserId);

        // Assert
        result.UserId.Should().Be(UserId);
        result.LibraryItems.Should().ContainSingle(i => i.MovieId == movie.Id);
    }

    [Fact]
    public async Task CreateLibraryEntryAsync_ValidInput_AddsLibraryEntryAndReturnsDto()
    {
        // Arrange
        var movie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();
        var dto = Fixture.Build<LibraryItemDto>().With(d => d.MovieId, movie.Id).Create();

        var existing = new List<UserMovie>().AsQueryable().BuildMockDbSet();
        _dbContext.UserMovies.Returns(existing);

        var added = new List<UserMovie>();
        _dbContext.UserMovies.When(x => x.Add(Arg.Any<UserMovie>())).Do(x => added.Add(x.Arg<UserMovie>()));
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _service.CreateLibraryEntryAsync(UserId, dto);

        // Assert
        result.MovieId.Should().Be(dto.MovieId);
        added.Should().ContainSingle(um => um.UserId == UserId && um.MovieId == dto.MovieId);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateLibraryEntryAsync_ExistingLibraryItem_UpdatesAndReturnsDto()
    {
        // Arrange
        var movie = Fixture.Build<Movie>().With(m => m.IsDeleted, false).Create();
        var userMovie = Fixture.Build<UserMovie>()
            .With(um => um.UserId, UserId)
            .With(um => um.MovieId, movie.Id)
            .With(um => um.Favourite, false)
            .With(um => um.Movie, movie)
            .Create();

        var mockSet = new List<UserMovie> { userMovie }.AsQueryable().BuildMockDbSet();
        _dbContext.UserMovies.Returns(mockSet);

        var dto = _mapper.Map<LibraryItemDto>(userMovie);
        dto.Favourite = true;

        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _service.UpdateLibraryEntryAsync(UserId, dto);

        // Assert
        result.Favourite.Should().BeTrue();
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
