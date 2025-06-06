using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IUsersService
{
    Task<IList<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int userId);
    Task<UserDto> CreateUserAsync(UserDto user);
    Task<UserDto> UpdateUserAsync(int userId, UserDto user);
    Task DeleteUserAsync(int userId);
}