using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.users;
using movielogger.api.models.responses.genres;
using movielogger.api.models.responses.movies;
using movielogger.api.models.responses.users;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

[Collection("IntegrationTests")]
public class UsersControllerTests : BaseTestController
{
    [Fact]
    public async Task GetAllUsers_ReturnAllUsers()
    {
        // Act
        var response = await _client.GetAsync("/users");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        
        users.Should().NotBeNull();
        users!.Should().HaveCount(2);
        users.Should().Contain(u => u.UserName == "John Doe");
        users.Should().Contain(u => u.UserName == "Jane Doe");
    }
    
    [Fact]
    public async Task GetUserById_WhenUserExists_ReturnsUser()
    {
        // Act
        var response = await _client.GetAsync("/users/1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        
        user.Should().NotBeNull();
        user!.Id.Should().Be(1);
        user.UserName.Should().Be("John Doe");
        user.Email.Should().Be("johndoe@example.com");
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/users/999");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateUser_WithValidData_ReturnsCreatedUser()
    {
        // Arrange 
        var request = new CreateUserRequest
        {
            UserName = "Jack Burton",
            Email = "jackburton@example.com",
            Password = "ItsAllInTheReflexes",
            IsAdmin = false,
            IsDeleted = false
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        
        createdUser.Should().NotBeNull();
        createdUser!.UserName.Should().Be(request.UserName);
        createdUser.Email.Should().Be(request.Email);
        createdUser.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateUser_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange 
        var request = new CreateUserRequest
        {
            UserName = "Invalid User",
            Email = "not-an-email",
            Password = "password123"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange 
        var request = new CreateUserRequest
        {
            UserName = "Duplicate Email",
            Email = "johndoe@example.com", // This email already exists
            Password = "password123"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateUser_WithValidData_ReturnsUpdatedUser()
    {
        // Arrange 
        var request = new UpdateUserRequest
        {
            UserName = "John Doe Updated",
            Email = "johndoeupdated@example.com",
            Password = "password123updated",
            IsAdmin = true,
            IsDeleted = false
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/users/1", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var updatedUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        
        updatedUser.Should().NotBeNull();
        updatedUser!.UserName.Should().Be(request.UserName);
        updatedUser.Email.Should().Be(request.Email);
        updatedUser.IsAdmin.Should().BeTrue();
        updatedUser.IsDeleted.Should().BeFalse();
        updatedUser.Id.Should().Be(1);
    }

    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange 
        var request = new UpdateUserRequest
        {
            UserName = "Non Existent",
            Email = "nonexistent@example.com",
            Password = "password123"
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/users/999", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ReturnsNoContent()
    {
        // Act
        var response = await _client.DeleteAsync("/users/1");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify user is deleted
        var getResponse = await _client.GetAsync("/users/1");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/users/999");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}