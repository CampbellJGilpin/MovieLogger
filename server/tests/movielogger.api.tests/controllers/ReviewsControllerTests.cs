using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
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
        var mockGenres = new ReviewDto[]
        {
            new()
            {
                Id = 1,
                DateViewed = DateTime.Now,
                MovieTitle = "Sinners",
                ReviewText = "Film was amazing.",
                Score = 5
            },
            new()
            {
                Id = 2,
                DateViewed = DateTime.Now,
                MovieTitle = "Captain America Brave New World",
                ReviewText = "Film was Ok.",
                Score = 3
            }
        }.ToList();

        _factory.ReviewsServiceMock.GetAllReviewsByUserIdAsync(5).Returns(mockGenres);
        
        // Act
        var response = await _client.GetAsync("/Reviews/5");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();
        
        Assert.NotNull(content);
        Assert.Equal(2, content.Count);
        
        Assert.Equal("Sinners", content[0].MovieTitle);
        Assert.Equal(5, content[0].Score);
        Assert.Equal("Film was amazing.", content[0].ReviewText);
        
        Assert.Equal("Captain America Brave New World", content[1].MovieTitle);
        Assert.Equal(3, content[1].Score);
        Assert.Equal("Film was Ok.", content[0].ReviewText);
    }
}