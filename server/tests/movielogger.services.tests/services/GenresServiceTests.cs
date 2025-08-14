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

public class GenresServiceTests : BaseServiceTest
{
    private readonly IGenresService _service;

    public GenresServiceTests()
    {
        _service = new GenresService(_dbContext, _mapper);
    }

    [Fact]
    public async Task GetGenresAsync_ReturnsMappedGenres()
    {
        // Arrange
        var genres = Fixture.CreateMany<Genre>(3).AsQueryable();
        var mockSet = genres.BuildMockDbSet();
        _dbContext.Genres.Returns(mockSet);

        // Act 
        var result = await _service.GetGenresAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ExistingGenre_ReturnsMappedGenre()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        _dbContext.Genres.FindAsync(genre.Id)!.Returns(new ValueTask<Genre>(genre));

        // Act 
        var result = await _service.GetGenreByIdAsync(genre.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(genre.Id);
        result.Title.Should().Be(genre.Title);
    }

    [Fact]
    public async Task GetGenreByIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _dbContext.Genres.FindAsync(Arg.Any<int>()).Returns((Genre?)null);

        // Act 
        var act = async () => await _service.GetGenreByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateGenreAsync_ValidDto_AddsAndReturnsMappedGenre()
    {
        // Arrange
        var genreDto = Fixture.Build<GenreDto>().With(g => g.Id, 1).Create();

        Genre? addedGenre = null;

        var mockSet = new List<Genre>().AsQueryable().BuildMockDbSet();
        mockSet.Add(Arg.Do<Genre>(g =>
        {
            g.Id = 1;
            addedGenre = g;
        }));

        _dbContext.Genres.Returns(mockSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act 
        var result = await _service.CreateGenreAsync(genreDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(genreDto.Title);

        addedGenre.Should().NotBeNull();
        addedGenre.Id.Should().Be(1);

        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateGenreAsync_ValidId_UpdatesAndReturnsMappedGenre()
    {
        // Arrange
        var genreDto = Fixture.Create<GenreDto>();
        var existingGenre = Fixture.Build<Genre>().With(g => g.Id, genreDto.Id).Create();

        _dbContext.Genres.FindAsync(existingGenre.Id)!.Returns(new ValueTask<Genre>(existingGenre));
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act 
        var result = await _service.UpdateGenreAsync(genreDto.Id, genreDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingGenre.Id);
        result.Title.Should().Be(genreDto.Title);
    }

    [Fact]
    public async Task UpdateGenreAsync_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _dbContext.Genres.FindAsync(Arg.Any<int>()).Returns((Genre?)null);

        var dto = Fixture.Create<GenreDto>();

        // Act 
        var act = async () => await _service.UpdateGenreAsync(123, dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteGenreAsync_ValidId_DeletesGenre()
    {
        // Arrange
        var genre = Fixture.Create<Genre>();
        _dbContext.Genres.FindAsync(genre.Id)!.Returns(new ValueTask<Genre>(genre));

        // Act 
        await _service.DeleteGenreAsync(genre.Id);

        // Assert
        _dbContext.Genres.Received(1).Remove(genre);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteGenreAsync_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _dbContext.Genres.FindAsync(Arg.Any<int>()).Returns((Genre?)null);

        // Act 
        var act = async () => await _service.DeleteGenreAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}