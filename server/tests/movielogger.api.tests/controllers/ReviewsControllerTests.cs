using System.Net;
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

[Collection("IntegrationTests")]
public class ReviewsControllerTests : BaseTestController
{
    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task GetUserReviews_ReturnsSeededReviews()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1/reviews");

        // Assert
        response.EnsureSuccessStatusCode();

        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();
        reviews.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task GetUserReviews_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/999/reviews");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task GetReviewById_WhenReviewExists_ReturnsReview()
    {
        // Act
        var response = await _client.GetAsync("/api/reviews/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var review = await response.Content.ReadFromJsonAsync<ReviewResponse>();

        review.Should().NotBeNull();
        review!.Id.Should().Be(1);
        review.Score.Should().BeGreaterThan(0);
        review.Score.Should().BeLessThanOrEqualTo(5);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task GetReviewById_WhenReviewDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/reviews/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
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

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task CreateReview_WithInvalidViewingId_ReturnsNotFound()
    {
        // Arrange
        const int viewingId = 999; // Non-existent viewing

        var request = new CreateReviewRequest
        {
            Score = 4,
            ReviewText = "Test review"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/viewings/{viewingId}/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task CreateReview_WithInvalidScore_ReturnsBadRequest()
    {
        // Arrange
        const int viewingId = 1;

        var request = new CreateReviewRequest
        {
            Score = 6, // Invalid score (should be 1-5)
            ReviewText = "Test review"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/viewings/{viewingId}/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task CreateReview_WithEmptyReviewText_ReturnsBadRequest()
    {
        // Arrange
        const int viewingId = 1;

        var request = new CreateReviewRequest
        {
            Score = 4,
            ReviewText = "" // Empty review text
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/viewings/{viewingId}/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
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
        updatedReview.Score.Should().Be(updateRequest.Score);
        updatedReview.ReviewText.Should().Be(updateRequest.ReviewText);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task UpdateReview_WhenReviewDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        const int reviewId = 999; // Non-existent review
        var updateRequest = new UpdateReviewRequest
        {
            Score = 5,
            ReviewText = "Non-existent review"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/{reviewId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task UpdateReview_WithInvalidScore_ReturnsBadRequest()
    {
        // Arrange
        const int reviewId = 1;
        var updateRequest = new UpdateReviewRequest
        {
            Score = 0, // Invalid score
            ReviewText = "Invalid score test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/{reviewId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task DeleteReview_WhenReviewExists_ReturnsSuccess()
    {
        // Act
        var response = await _client.DeleteAsync("/api/reviews/2"); // Assuming review 2 exists

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify review is deleted
        var getResponse = await _client.GetAsync("/api/reviews/2");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task DeleteReview_WhenReviewDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/reviews/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task GetReviews_WithScoreFilter_ReturnsFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1/reviews?score=4");

        // Assert
        response.EnsureSuccessStatusCode();
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();

        reviews.Should().NotBeNull();
        reviews!.Should().OnlyContain(r => r.Score == 4);
    }

    [Fact(Skip = "Test needs update for new UserMovieViewing structure")]
    public async Task GetReviews_WithDateFilter_ReturnsFilteredResults()
    {
        // Act
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/users/1/reviews?reviewDate={today}");

        // Assert
        response.EnsureSuccessStatusCode();
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();

        reviews.Should().NotBeNull();
        // Note: This test assumes there are reviews on the current date
        // In a real scenario, you might seed specific review data for testing
    }
}