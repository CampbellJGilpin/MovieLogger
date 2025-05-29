using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.viewings;
using movielogger.api.models.responses.viewings;

namespace movielogger.api.tests.controllers;

public class ViewingsControllerTests : BaseTestController
{
    [Fact]
    public async Task GetViewing_ReturnsViewing()
    {
        // Act
        var response = await _client.GetAsync($"/viewings/1");
        
        // Assert
        response.EnsureSuccessStatusCode();

        var viewing = response.Content.ReadFromJsonAsync<ViewingResponse>();
        viewing.Id.Should().Be(1);
    }
    
    [Fact]
    public async Task CreateViewing_ReturnsCreatedViewing()
    {
        // Arrange
        var request = new CreateViewingRequest
        { 
            MovieId = 1,
            DateViewed = DateTime.Now.AddDays(-1)
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/viewings", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdViewing = response.Content.ReadFromJsonAsync<ViewingResponse>();

        createdViewing.Should().NotBeNull();
    }
    
    [Fact]
    public async Task UpdateViewing_ReturnsUpdatedViewing()
    {
        // Arrange
        var request = new UpdateViewingRequest
        {
            DateViewed = DateTime.Now.AddMonths(-1)
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/viewings/1", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
}