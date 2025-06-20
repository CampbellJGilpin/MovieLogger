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
}