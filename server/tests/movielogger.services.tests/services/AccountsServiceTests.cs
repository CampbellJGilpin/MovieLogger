using AutoMapper;
using movielogger.dal;
using movielogger.services.interfaces;
using movielogger.services.services;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MockQueryable.NSubstitute;
using movielogger.dal.entities;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ReturnsExtensions;
using System.Threading;

namespace movielogger.services.tests.services;

public class AccountsServiceTests : BaseServiceTest
{
    private readonly IConfiguration _configuration;
    private readonly AccountsService _service;

    public AccountsServiceTests() : base()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "your-super-secret-key-that-is-long-enough" },
            { "Jwt:Issuer", "test-issuer" },
            { "Jwt:Audience", "test-audience" },
            { "Jwt:ExpiresInMinutes", "60" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _service = new AccountsService(_dbContext, _mapper, _configuration);
    }

    [Fact]
    public async Task Register_ValidCredentials_CreatesAndReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var userName = "testuser";

        var users = new List<User>();
        var mockSet = users.AsQueryable().BuildMockDbSet();

        mockSet.When(x => x.Add(Arg.Any<User>())).Do(x => users.Add(x.Arg<User>()));

        _dbContext.Users.Returns(mockSet);

        // Act
        var result = await _service.Register(email, password, userName);

        // Assert
        await _dbContext.Received(1).SaveChangesAsync();

        users.Should().HaveCount(1);
        var capturedUser = users.First();

        capturedUser.Email.Should().Be(email);
        capturedUser.UserName.Should().Be(userName);
        BC.Verify(password, capturedUser.Password).Should().BeTrue();

        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.UserName.Should().Be(userName);
    }

    [Fact]
    public async Task Register_EmailAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "existing@example.com";
        var password = "password123";
        var userName = "newuser";

        var existingUser = Fixture.Build<User>().With(u => u.Email, email).Create();
        var users = new List<User> { existingUser };
        var mockSet = users.AsQueryable().BuildMockDbSet();
        _dbContext.Users.Returns(mockSet);

        // Act
        var act = () => _service.Register(email, password, userName);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Email already exists");
    }

    [Fact]
    public async Task AuthenticateUserAsync_ValidCredentials_ReturnsUserAndToken()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var hashedPassword = BC.HashPassword(password);
        var user = Fixture.Build<User>()
            .With(u => u.Email, email)
            .With(u => u.Password, hashedPassword)
            .Create();

        var users = new List<User> { user }.AsQueryable().BuildMockDbSet();
        _dbContext.Users.Returns(users);

        // Act
        var result = await _service.AuthenticateUserAsync(email, password);

        // Assert
        result.user.Should().NotBeNull();
        result.user.Email.Should().Be(email);
        result.token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AuthenticateUserAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        var hashedPassword = BC.HashPassword("correctpassword");
        var user = Fixture.Build<User>()
            .With(u => u.Email, email)
            .With(u => u.Password, hashedPassword)
            .Create();

        var users = new List<User> { user }.AsQueryable().BuildMockDbSet();
        _dbContext.Users.Returns(users);

        // Act
        var act = () => _service.AuthenticateUserAsync(email, password);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidInput_ChangesPassword()
    {
        // Arrange
        var userId = 1;
        var currentPassword = "oldpassword";
        var newPassword = "newpassword";
        var hashedCurrentPassword = BC.HashPassword(currentPassword);
        var user = Fixture.Build<User>()
            .With(u => u.Id, userId)
            .With(u => u.Password, hashedCurrentPassword)
            .Create();

        var users = new List<User> { user }.AsQueryable().BuildMockDbSet();
        _dbContext.Users.Returns(users);

        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _service.ChangePasswordAsync(userId, currentPassword, newPassword);

        // Assert
        BC.Verify(newPassword, user.Password).Should().BeTrue();
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ChangePasswordAsync_InvalidCurrentPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = 1;
        var currentPassword = "wrongpassword";
        var newPassword = "newpassword";
        var hashedCurrentPassword = BC.HashPassword("correctpassword");
        var user = Fixture.Build<User>()
            .With(u => u.Id, userId)
            .With(u => u.Password, hashedCurrentPassword)
            .Create();

        var users = new List<User> { user }.AsQueryable().BuildMockDbSet();
        _dbContext.Users.Returns(users);

        // Act
        var act = () => _service.ChangePasswordAsync(userId, currentPassword, newPassword);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}