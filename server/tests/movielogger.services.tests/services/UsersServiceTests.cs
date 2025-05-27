using AutoFixture;
using FluentAssertions;
using MockQueryable.NSubstitute;
using movielogger.dal.dtos;
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
        _dbContext.Users.FindAsync(Arg.Any<int>()).Returns((User?)null);

        // Act 
        var act = async () => await _service.GetUserByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
    
    [Fact]
    public async Task CreateUserAsync_ValidDto_AddsAndReturnsMappedUser()
    {
        // Arrange
        var userDto = Fixture.Build<UserDto>()
            .With(g => g.Id, 1)
            .With(x => x.IsDeleted, false)
            .Create();
        
        User? addedUser = null;

        var mockSet = new List<User>().AsQueryable().BuildMockDbSet();
        mockSet.Add(Arg.Do<User>(u => {
            u.Id = 1;
            addedUser = u;
        }));
        
        _dbContext.Users.Returns(mockSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        
        // Act 
        var result = await _service.CreateUserAsync(userDto);
        
        // Assert
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        mockSet.Received(1).Add(Arg.Any<User>());
        
        result.Should().NotBeNull();
        result.Id.Should().Be(userDto.Id);
        result.UserName.Should().Be(userDto.UserName);
        result.Email.Should().Be(userDto.Email);
        result.IsDeleted.Should().BeFalse();
    }
    
    [Fact]
    public async Task UpdateUserAsync_ValidId_UpdatesAndReturnsMappedUser()
    {
        // Arrange
        var userDto = Fixture.Build<UserDto>()
            .With(g => g.Id, 1)
            .With(x => x.IsDeleted, false)
            .Create();

        var existingUser = Fixture.Build<User>()
            .With(g => g.Id, userDto.Id)
            .With(x => x.IsDeleted, false)
            .Create();

        var users = new List<User> { existingUser }.AsQueryable();
        var mockSet = users.BuildMockDbSet();

        _dbContext.Users.Returns(mockSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        
        // Act 
        var result = await _service.UpdateUserAsync(userDto.Id, userDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userDto.Id);
        result.UserName.Should().Be(userDto.UserName);
        result.Email.Should().Be(userDto.Email);
        result.IsDeleted.Should().BeFalse();
    }
    
    [Fact]
    public async Task UpdateUserAsync_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _dbContext.Users.FindAsync(Arg.Any<int>()).Returns((User?)null);

        var dto = Fixture.Create<UserDto>();
        
        // Act 
        var act = async () => await _service.UpdateUserAsync(123, dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}