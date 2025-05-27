using AutoFixture;
using FluentAssertions;
using MockQueryable.NSubstitute;
using movielogger.dal.entities;
using movielogger.services.interfaces;
using movielogger.services.services;
using NSubstitute;

namespace movielogger.services.tests.services;

public class UsersServiceTests : BaseServiceTest
{
    IUsersService _service;
    
    public UsersServiceTests()
    {
        _service = new UsersService(_dbContext, _mapper);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsMappedUsers()
    {
        // Arrange
        var users = Fixture.Build<User>()
            .With(x => x.IsDeleted, false)
            .CreateMany(4)
            .ToList();
        var mockSet = users.BuildMockDbSet();
        _dbContext.Users.Returns(mockSet);
        
        // Act 
        var result = await _service.GetAllUsersAsync();
        
        // Assert
        result.Should().HaveCount(4);
    }
    
    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ReturnsMappedUser()
    {
        // Arrange
        var user = Fixture.Build<User>()
            .With(x => x.IsDeleted, false)
            .Create();
        
        var users = new List<User> { user }.AsQueryable();
        var mockSet = users.BuildMockDbSet();
        
        _dbContext.Users.Returns(mockSet);
        
        // Act 
        var result = await _service.GetUserByIdAsync(user.Id);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.UserName.Should().Be(user.UserName);
        result.Email.Should().Be(user.Email);
    }
    
    [Fact]
    public async Task GetUserByIdAsync_NotFound_ReturnsMappedUser()
    {
        // Arrange
        
        // Act 
        
        // Assert
    }
    
    [Fact]
    public async Task CreateUserAsync_ValidDto_AddsAndReturnsMappedUser()
    {
        // Arrange
        
        // Act 
        
        // Assert
    }
    
    [Fact]
    public async Task UpdateUserAsync_ValidId_UpdatesAndReturnsMappedUser()
    {
        // Arrange
        
        // Act 
        
        // Assert
    }
    
    [Fact]
    public async Task UpdateUserAsync_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        
        // Act 
        
        // Assert
    }
}