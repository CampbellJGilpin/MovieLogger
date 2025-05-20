using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.reviews;
using movielogger.api.models.responses.genres;
using movielogger.api.models.responses.reviews;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class ReviewsControllerTests  : BaseTestController
{
    [Fact]
    public async Task GetAllReviews_ReturnsSuccess()
    {
        // Arrange 
        const string firstMovieTitle = "Movie 1";
        const int firstReviewId = 1;
        const string firstReviewText = "Review 1";
        const int firstMovieScore = 5;
        var firstViewDate = new DateTime(2021, 1, 1);
        
        const string secondMovieTitle = "Movie 2";
        const int secondReviewId = 2;
        const string secondReviewText = "Review 2";
        const int secondMovieScore = 4;
        var secondViewDate = new DateTime(2022, 2, 2);
        
        var mockGenres = new ReviewDto[]
        {
            new()
            {
                Id = firstReviewId,
                DateViewed = firstViewDate,
                MovieTitle = firstMovieTitle,
                ReviewText = firstReviewText,
                Score = firstMovieScore
            },
            new()
            {
                Id = secondReviewId,
                DateViewed = secondViewDate,
                MovieTitle = secondMovieTitle,
                ReviewText = secondReviewText,
                Score = secondMovieScore
            }
        }.ToList();
        

        _factory.ReviewsServiceMock.GetAllReviewsByUserIdAsync(5).Returns(mockGenres);
        
        // Act
        var response = await _client.GetAsync("/Users/5/Reviews");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();
        
        Assert.NotNull(content);
        Assert.Equal(2, content.Count);
        
        Assert.Equal(firstMovieScore, content[0].Score);
        Assert.Equal(firstMovieTitle, content[0].MovieTitle);
        Assert.Equal(firstReviewId, content[0].Id);
        Assert.Equal(firstReviewText, content[0].ReviewText);
        Assert.Equal(firstViewDate, content[0].DateViewed);
        
        Assert.Equal(secondMovieScore, content[1].Score);
        Assert.Equal(secondMovieTitle, content[1].MovieTitle);
        Assert.Equal(secondReviewId, content[1].Id);
        Assert.Equal(secondReviewText, content[1].ReviewText);
        Assert.Equal(secondViewDate, content[1].DateViewed);
    }
    
    [Fact]
    public async Task Post_WithValidData_SavesReview()
    {
        // Arrange 
        const string reviewText = "Review 3";
        const int reviewScore = 1;
        const int reviewId = 3;
        const string movieTitle = "Movie 3";
        var viewDate = new DateTime(2023, 3, 3);

        var viewingId = 1;
        
        var newReview = new CreateReviewRequest { ReviewText = reviewText, Score = reviewScore };
        
        var mockReview = new ReviewDto
        {
            DateViewed = viewDate,
            Id = reviewId,
            MovieTitle = movieTitle,
            ReviewText = reviewText,
            Score = reviewScore
        };
        
        _factory.ReviewsServiceMock.CreateReviewAsync(Arg.Any<ReviewDto>()).Returns(mockReview);
        
        // Act
        var response = await _client.PostAsync($"Viewings/{viewingId}/Reviews", JsonContent.Create(newReview));

        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<ReviewResponse>();
        
        Assert.NotNull(content);
        Assert.Equal(movieTitle, content.MovieTitle);
        Assert.Equal(reviewId, content.Id);
        Assert.Equal(reviewScore, content.Score);
        Assert.Equal(reviewText, content.ReviewText);
        Assert.Equal(viewDate, content.DateViewed);
    }
}