using AutoFixture;
using FluentAssertions;
using MockQueryable.NSubstitute;
using movielogger.dal.dtos;
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
        var viewingId = 1;
        var reviewDto = Fixture.Build<ReviewDto>().With(x => x.Id, 1).Create();

        Review? addedReview = null;

        var mockSet = new List<Review>().AsQueryable().BuildMockDbSet();
        mockSet.Add(Arg.Do<Review>(u =>
        {
            u.Id = reviewDto.Id;
            addedReview = u;
        }));

        _dbContext.Reviews.Returns(mockSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        
        // Act
        var result = await _service.CreateReviewAsync(viewingId, reviewDto);
        
        // Assert
        result.Should().NotBeNull();
        result.ReviewText.Should().Be(reviewDto.ReviewText);
        
        addedReview.Should().NotBeNull();
        addedReview.Id.Should().Be(1);
        
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReviewAsync_ExistingReview_UpdatesAndReturnsMappedDto()
    {
        // Arrange
        var reviewDto = Fixture.Build<ReviewDto>()
            .With(x => x.Id, 1)
            .Create();
    
        var existingReview = Fixture.Build<Review>()
            .With(x => x.Id, reviewDto.Id)
            .Create();
    
        var reviews = new List<Review> { existingReview }.AsQueryable();
        var mockSet = reviews.BuildMockDbSet();
        
        _dbContext.Reviews.Returns(mockSet);
        _dbContext.Reviews.FindAsync(reviewDto.Id).Returns(new ValueTask<Review>(existingReview));

        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        
        // Act
        var result = await _service.UpdateReviewAsync(reviewDto.Id, reviewDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(reviewDto.Id);
        result.ReviewText.Should().Be(reviewDto.ReviewText);
    }
}