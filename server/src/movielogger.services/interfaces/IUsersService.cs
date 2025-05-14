using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IUsersService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int userId);
    Task<UserDto> CreateUserAsync(UserDto userDto);
    Task<UserDto> UpdateUserAsync(int userId, UserDto userDto);
}