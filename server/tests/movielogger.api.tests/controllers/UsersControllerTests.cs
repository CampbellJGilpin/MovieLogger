using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.responses.movies;
using movielogger.api.models.responses.users;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class UsersControllerTests  : BaseTestController
{
    [Fact]
    public async Task Get_Always_ReturnsAllMovies()
    {
        const int firstUserId = 1;
        const string firstUserName = "Username 1";
        const string firstEmail = "email1@email.com";
        const string firstPassword = "password1";
        
        const int secondUserId = 2;
        const string secondUserName = "Username 2";
        const string secondEmail = "email2@email.com";
        const string secondPassword = "password2";
        
        // Arrange 
        var mockUsers = new UserDto[]
        {
            new()
            {
                Id = firstUserId,
                UserName = firstUserName,
                Email = firstEmail,
                Password = firstPassword,
            },
            new()
            {
                Id = secondUserId,
                UserName = secondUserName,
                Email = secondEmail,
                Password = secondPassword,
            }
        }.ToList();

        _factory.UsersService.GetAllUsersAsync().Returns(mockUsers);
        
        // Act
        var response = await _client.GetAsync("/Users");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        
        Assert.NotNull(content);
        Assert.Equal(2, content.Count);
        
        Assert.Equal(firstUserId, content[0].Id);
        Assert.Equal(firstUserName, content[0].UserName);
        Assert.Equal(firstEmail, content[0].Email);
        
        Assert.Equal(secondUserId, content[1].Id);
        Assert.Equal(secondUserName, content[1].UserName);
        Assert.Equal(secondEmail, content[1].Email);
    }
    
    [Fact]
    public async Task GetById_IfExists_ReturnsAllMovies()
    {
        const int userId = 1;
        const string userName = "Username 1";
        const string email = "email1@email.com";
        const string password = "password1";
        
        // Arrange 
        var mockUser = new UserDto { Id = userId, UserName = userName, Email = email, Password = password };

        _factory.UsersService.GetUserByIdAsync(userId).Returns(mockUser);
        
        // Act
        var response = await _client.GetAsync($"/Users/{userId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        
        Assert.NotNull(content);
        
        Assert.Equal(userId, content.Id);
        Assert.Equal(userName, content.UserName);
        Assert.Equal(email, content.Email);
    }
}