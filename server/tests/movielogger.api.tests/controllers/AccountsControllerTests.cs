using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.users;
using movielogger.api.models.responses.users;

namespace movielogger.api.tests.controllers;

[Collection("IntegrationTests")]
public class AccountsControllerTests : BaseTestController
{
    [Fact]
    public async Task Register_WithValidData_ReturnsSuccess()
    {
        // Arrange 
        var request = new RegisterRequest
        {
            UserName = "New User",
            Email = "newuser@example.com",
            Password = "SecurePassword123!"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        var user = result.GetProperty("user").Deserialize<UserResponse>();
        
        user.Should().NotBeNull();
        user!.UserName.Should().Be(request.UserName);
        user.Email.Should().Be(request.Email);
        user.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange 
        var request = new RegisterRequest
        {
            UserName = "Duplicate User",
            Email = "johndoe@example.com", // This email already exists
            Password = "SecurePassword123!"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange 
        var request = new RegisterRequest
        {
            UserName = "Invalid User",
            Email = "not-an-email",
            Password = "SecurePassword123!"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange 
        var request = new RegisterRequest
        {
            UserName = "Weak Password User",
            Email = "weakpassword@example.com",
            Password = "123" // Too short
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange 
        var request = new LoginRequest
        {
            Email = "johndoe@example.com",
            Password = "password123"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        loginResponse.Should().NotBeNull();
        loginResponse!.Token.Should().NotBeNullOrEmpty();
        loginResponse.User.Should().NotBeNull();
        loginResponse.User.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange 
        var request = new LoginRequest
        {
            Email = "johndoe@example.com",
            Password = "wrongpassword"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        // Arrange 
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WithValidData_ReturnsSuccess()
    {
        // Arrange 
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "password123",
            NewPassword = "NewSecurePassword123!"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/change-password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_WithInvalidOldPassword_ReturnsBadRequest()
    {
        // Arrange 
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "wrongpassword",
            NewPassword = "NewSecurePassword123!"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/change-password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_WithWeakNewPassword_ReturnsBadRequest()
    {
        // Arrange 
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "password123",
            NewPassword = "123" // Too short
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/accounts/change-password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}