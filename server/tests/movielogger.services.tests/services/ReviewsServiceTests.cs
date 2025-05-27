using AutoFixture;
using FluentAssertions;
using MockQueryable.NSubstitute;
using movielogger.dal.entities;
using movielogger.services.interfaces;
using movielogger.services.services;
using NSubstitute;

namespace movielogger.services.tests.services;

public class ReviewsServiceTests : BaseServiceTest
{
    IReviewsService _service;
    
    public ReviewsServiceTests()
    {
        _service = new ReviewsService(_dbContext, _mapper);
    }
    
    [Fact]
    public async Task GetAllReviewsByUserIdAsync_ReturnsMappedReviews()
    {
        // Arrange
        var userId = 5;

        var movie = Fixture.Create<Movie>();
        
        var userMovie = Fixture.Build<UserMovie>()
            .With(um => um.UserId, userId)
            .With(um => um.Movie, movie)
            .Create();
        
        var viewing = Fixture.Build<Viewing>()
            .With(v => v.UserMovie, userMovie)
            .Create();
        
        var reviews = Fixture.Build<Review>()
            .With(r => r.Viewing, viewing)
            .CreateMany(3)
            .AsQueryable();

        var mockSet = reviews.BuildMockDbSet();
        _dbContext.Reviews.Returns(mockSet);

        // Act
        var result = await _service.GetAllReviewsByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task CreateReviewAsync_ValidInput_AddsReviewAndReturnsDto()
    {
        // Arrange
        
        // Act
        
        // Assert
    }

    [Fact]
    public async Task UpdateReviewAsync_ExistingReview_UpdatesAndReturnsMappedDto()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
}