using System.Net.Http.Json;
using FluentAssertions;
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
    public async Task GetUserReviews_ReturnsSeededReviews()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1/reviews");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();
        reviews.Should().HaveCountGreaterThanOrEqualTo(2);
    }
    
    [Fact]
    public async Task CreateReview_ReturnsCreatedReview()
    {
        // Arrange
        const int viewingId = 1;
        
        var request = new CreateReviewRequest
        {
            Score = 4,
            ReviewText = "Really good movie!"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/viewings/{viewingId}/reviews", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdReview = await response.Content.ReadFromJsonAsync<ReviewResponse>();

        createdReview.Should().NotBeNull();
        createdReview!.Score.Should().Be(request.Score);
        createdReview.ReviewText.Should().Be(request.ReviewText);
        createdReview.Id.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task UpdateReview_ReturnsUpdatedReview()
    {
        // Arrange
        const int reviewId = 1;
        var updateRequest = new UpdateReviewRequest
        {
            Score = 5,
            ReviewText = "Actually, it was amazing!"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/{reviewId}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedReview = await response.Content.ReadFromJsonAsync<ReviewResponse>();

        updatedReview.Should().NotBeNull();
        updatedReview!.Id.Should().Be(reviewId);
        updatedReview!.Score.Should().Be(updatedReview.Score);
        updatedReview.ReviewText.Should().Be(updatedReview.ReviewText);
    }
}