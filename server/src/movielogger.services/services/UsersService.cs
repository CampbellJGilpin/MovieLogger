using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.dal.Exceptions;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class UsersService : IUsersService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public UsersService(IAssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IList<UserDto>> GetAllUsersAsync()
    {
        var users = await _db.Users
            .Where(u => !u.IsDeleted)
            .ToListAsync();
        return _mapper.Map<IList<UserDto>>(users);
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(UserDto userDto)
    {
        var existingUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == userDto.Email);

        if (existingUser != null)
        {
            throw new EmailAlreadyExistsException();
        }

        var user = _mapper.Map<User>(userDto);
        user.Id = 0;

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserAsync(int userId, UserDto userDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        _mapper.Map(userDto, user);
        user.Id = userId; // Explicitly ensure the correct ID remains

        await _db.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        user.IsDeleted = true;
        await _db.SaveChangesAsync();
    }
}