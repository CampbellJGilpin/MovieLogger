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

public class UsersControllerTests  : BaseTestController
{
    [Fact]
    public async Task GetAllUsers_ReturnAllUsers()
    {
        // Act
        var response = await _client.GetAsync("/users");
        
        // Assert
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        users.Should().HaveCountGreaterThanOrEqualTo(2);
    }
    
    [Fact]
    public async Task GetUserById_ReturnsUser()
    {
        // Act
        var response = await _client.GetAsync("/users/1");
        
        // Assert
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(1);
        user.UserName.Should().Be("John Doe");
    }
    
    [Fact]
    public async Task CreateUser_ReturnsCreatedUser()
    {
        // Arrange 
        var request = new CreateUserRequest
        {
            UserName = "Jack Burton",
            Email = "jackburton@gmail.com",
            Password = "ItsAllInTheReflexes",
            IsAdmin = false,
            IsDeleted = false
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/users", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdGenre = await response.Content.ReadFromJsonAsync<UserResponse>();
        
        createdGenre.Should().NotBeNull();
        createdGenre.UserName.Should().Be("Jack Burton");
        createdGenre.Email.Should().Be("jackburton@gmail.com");
        createdGenre.Id.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task UpdateUser_ReturnsUpdatedUser()
    {
        // Arrange 
        var request = new UpdateUserRequest
        {
            UserName = "John Doe Updated",
            Email = "johndoeupdated@gmail.com",
            Password = "password123updated",
            IsAdmin = true,
            IsDeleted = true
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/users/1", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdGenre = await response.Content.ReadFromJsonAsync<UserResponse>();
        
        createdGenre.Should().NotBeNull();
        createdGenre.UserName.Should().Be("John Doe Updated");
        createdGenre.Email.Should().Be("johndoeupdated@gmail.com");
        createdGenre.IsAdmin.Should().BeTrue();
        createdGenre.IsDeleted.Should().BeTrue();
        createdGenre.Id.Should().Be(1);
    }
}