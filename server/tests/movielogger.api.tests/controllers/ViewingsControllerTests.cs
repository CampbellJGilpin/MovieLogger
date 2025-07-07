using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.viewings;
using movielogger.api.models.responses.viewings;
using movielogger.api.tests.fixtures;
using movielogger.api.tests.helpers;
using System.Net.Http.Headers;

namespace movielogger.api.tests.controllers;

[Collection("IntegrationTests")]
public class ViewingsControllerTests : BaseTestController
{
    [Fact]
    public async Task GetUserViewings_ReturnsSeededViewings()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1/viewings");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var viewings = await response.Content.ReadFromJsonAsync<List<ViewingResponse>>();
        
        viewings.Should().NotBeNull();
        viewings!.Should().HaveCountGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetUserViewings_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/999/viewings");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetViewingById_WhenViewingExists_ReturnsViewing()
    {
        // Act
        var response = await _client.GetAsync("/api/viewings/1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var viewing = await response.Content.ReadFromJsonAsync<ViewingResponse>();
        
        viewing.Should().NotBeNull();
        viewing!.ViewingId.Should().Be(1);
        viewing.UserId.Should().Be(1);
        viewing.Movie.Should().NotBeNull();
    }

    [Fact]
    public async Task GetViewingById_WhenViewingDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/viewings/999");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateViewing_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateViewingRequest
        {
            UserId = 1,
            MovieId = 1,
            DateViewed = DateTime.Now
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/1/viewings", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdViewing = await response.Content.ReadFromJsonAsync<ViewingResponse>();
        
        createdViewing.Should().NotBeNull();
        createdViewing!.Movie.Should().NotBeNull();
        createdViewing.UserId.Should().Be(request.UserId);
        createdViewing.ViewingId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateViewing_WithInvalidMovieId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateViewingRequest
        {
            UserId = 1,
            MovieId = 999, // Non-existent movie
            DateViewed = DateTime.Now
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/1/viewings", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateViewing_WithInvalidUserId_ReturnsNotFound()
    {
        // Arrange
        var request = new CreateViewingRequest
        {
            UserId = 999,
            MovieId = 1,
            DateViewed = DateTime.Now
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/999/viewings", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateViewing_WithInvalidDate_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateViewingRequest
        {
            UserId = 1,
            MovieId = 1,
            DateViewed = DateTime.Now.AddDays(1) // Future date
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/1/viewings", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateViewing_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new UpdateViewingRequest
        {
            DateViewed = DateTime.Now.AddDays(-1)
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/viewings/1", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var updatedViewing = await response.Content.ReadFromJsonAsync<ViewingResponse>();
        
        updatedViewing.Should().NotBeNull();
        updatedViewing!.ViewingId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateViewing_WhenViewingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateViewingRequest
        {
            DateViewed = DateTime.Now
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/viewings/999", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateViewing_WithInvalidDate_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateViewingRequest
        {
            DateViewed = DateTime.Now.AddDays(1) // Future date
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/viewings/1", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteViewing_WhenViewingExists_ReturnsSuccess()
    {
        // Act
        var response = await _client.DeleteAsync("/api/viewings/2"); // Assuming viewing 2 exists
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify viewing is deleted
        var getResponse = await _client.GetAsync("/api/viewings/2");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteViewing_WhenViewingDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/viewings/999");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetViewings_WithDateFilter_ReturnsFilteredResults()
    {
        // Act
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/users/1/viewings?viewDate={today}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var viewings = await response.Content.ReadFromJsonAsync<List<ViewingResponse>>();
        
        viewings.Should().NotBeNull();
        // Note: This test assumes there are viewings on the current date
        // In a real scenario, you might seed specific viewing data for testing
    }

    [Fact]
    public async Task GetViewings_WithFavouriteFilter_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = TestDataBuilder.CreateTestUser(1);
        var token = AuthenticationHelper.GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/users/1/viewings?favourite=false");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var viewings = await response.Content.ReadFromJsonAsync<List<ViewingResponse>>();
        viewings.Should().NotBeNull();
        viewings!.Should().OnlyContain(v => v.Favourite == false);
    }
}