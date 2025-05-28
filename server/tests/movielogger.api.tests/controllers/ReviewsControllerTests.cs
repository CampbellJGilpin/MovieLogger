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
        var response = await _client.GetAsync("/users/1/reviews");
        
        response.EnsureSuccessStatusCode();
        
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();
        reviews.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}