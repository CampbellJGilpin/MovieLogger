using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class UsersService : IUsersService
{
    public Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> CreateUserAsync(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> UpdateUserAsync(int userId, UserDto userDto)
    {
        throw new NotImplementedException();
    }
}