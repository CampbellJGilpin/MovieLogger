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

public class ViewingsServiceTests : BaseServiceTest
{
    IViewingsService _service;
    
    public ViewingsServiceTests()
    {
        _service = new ViewingsService(_dbContext, _mapper);
    }

    [Fact]
    public async Task GetViewingByIdAsync_ValidId_ReturnsMappedViewing()
    {
        // Arrange
        var movie = Fixture.Build<Movie>()
            .With(x => x.IsDeleted, false)
            .Create();

        var userMovie = Fixture.Build<UserMovie>()
            .With(x => x.Movie, movie)
            .With(x => x.MovieId, movie.Id)
            .Create();

        var viewing = Fixture.Build<Viewing>()
            .With(x => x.UserMovie, userMovie)
            .Create();
        
        var viewings = new List<Viewing> {viewing}.AsQueryable();

        var mockSet = viewings.BuildMockDbSet();
        _dbContext.Viewings.Returns(mockSet);
        
        // Act
        var result = await _service.GetViewingByIdAsync(viewing.Id);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(viewing.Id);
        result.MovieId.Should().Be(movie.Id);
        result.Movie!.Title.Should().Be(movie.Title);
    }
}