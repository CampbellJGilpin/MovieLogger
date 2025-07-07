using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.library;
using movielogger.api.models.responses.library;

namespace movielogger.api.tests.controllers;

[Collection("IntegrationTests")]
public class LibraryControllerTests : BaseTestController
{
    [Fact]
    public async Task GetUserLibrary_ReturnsUserMovies()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1/library");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var library = await response.Content.ReadFromJsonAsync<LibraryResponse>();
        
        library.Should().NotBeNull();
        library!.LibraryItems.Should().NotBeNull();
        library.LibraryItems.Should().HaveCountGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetUserLibrary_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/999/library");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddMovieToLibrary_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateLibraryItemRequest
        {
            MovieId = 1,
            IsFavorite = true,
            OwnsMovie = false,
            UpcomingViewDate = DateTime.Now.AddDays(7)
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/1/library", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var libraryItem = await response.Content.ReadFromJsonAsync<LibraryItemResponse>();
        
        libraryItem.Should().NotBeNull();
        libraryItem!.MovieId.Should().Be(request.MovieId);
        libraryItem.Favourite.Should().Be("true"); // String representation
        libraryItem.InLibrary.Should().Be("false"); // String representation
    }

    [Fact]
    public async Task AddMovieToLibrary_WithInvalidMovieId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateLibraryItemRequest
        {
            MovieId = 999, // Non-existent movie
            IsFavorite = false,
            OwnsMovie = false
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/1/library", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMovieToLibrary_WithInvalidUserId_ReturnsNotFound()
    {
        // Arrange
        var request = new CreateLibraryItemRequest
        {
            MovieId = 1,
            IsFavorite = false,
            OwnsMovie = false
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/999/library", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveMovieFromLibrary_WithValidData_ReturnsSuccess()
    {
        // First, add a movie to the library
        var addRequest = new CreateLibraryItemRequest
        {
            MovieId = 2,
            IsFavorite = false,
            OwnsMovie = false
        };
        
        await _client.PostAsJsonAsync("/api/users/1/library", addRequest);
        
        // Act - Remove the movie from library
        var response = await _client.DeleteAsync("/api/users/1/library/2");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify the movie is removed
        var libraryResponse = await _client.GetAsync("/api/users/1/library");
        libraryResponse.EnsureSuccessStatusCode();
        var library = await libraryResponse.Content.ReadFromJsonAsync<LibraryResponse>();
        
        library!.LibraryItems.Should().NotContain(item => item.MovieId == 2);
    }

    [Fact]
    public async Task RemoveMovieFromLibrary_WhenMovieNotInLibrary_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/users/1/library/999");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateLibraryItem_WithValidData_ReturnsSuccess()
    {
        // First, add a movie to the library
        var addRequest = new CreateLibraryItemRequest
        {
            MovieId = 3,
            IsFavorite = false,
            OwnsMovie = false
        };
        
        await _client.PostAsJsonAsync("/api/users/1/library", addRequest);
        
        // Arrange - Update the library item
        var updateRequest = new UpdateLibraryItemRequest
        {
            IsFavorite = true,
            OwnsMovie = true,
            UpcomingViewDate = DateTime.Now.AddDays(14)
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/users/1/library/3", updateRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var updatedItem = await response.Content.ReadFromJsonAsync<LibraryItemResponse>();
        
        updatedItem.Should().NotBeNull();
        updatedItem!.Favourite.Should().Be("true"); // String representation
        updatedItem.InLibrary.Should().Be("true"); // String representation
        updatedItem.MovieId.Should().Be(3);
    }

    [Fact]
    public async Task UpdateLibraryItem_WhenMovieNotInLibrary_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new UpdateLibraryItemRequest
        {
            IsFavorite = true,
            OwnsMovie = false
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/users/1/library/999", updateRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetLibraryItem_WhenItemExists_ReturnsItem()
    {
        // First, add a movie to the library
        var addRequest = new CreateLibraryItemRequest
        {
            MovieId = 4,
            IsFavorite = true,
            OwnsMovie = false
        };
        
        await _client.PostAsJsonAsync("/api/users/1/library", addRequest);
        
        // Act
        var response = await _client.GetAsync("/api/users/1/library/4");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var libraryItem = await response.Content.ReadFromJsonAsync<LibraryItemResponse>();
        
        libraryItem.Should().NotBeNull();
        libraryItem!.MovieId.Should().Be(4);
        libraryItem.Favourite.Should().Be("true");
    }

    [Fact]
    public async Task GetLibraryItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1/library/999");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}