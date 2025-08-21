using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.users;
using movielogger.api.models.responses.users;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.dal.Exceptions;
using movielogger.services.interfaces;
using BC = BCrypt.Net.BCrypt;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(IUsersService usersService, IAccountsService accountsService, IMapper mapper) : ControllerBase
    {
        private readonly IUsersService _usersService = usersService;
        private readonly IAccountsService _accountsService = accountsService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var serviceResponse = await _usersService.GetAllUsersAsync();
            var mappedResponse = _mapper.Map<IList<UserResponse>>(serviceResponse);

            return Ok(mappedResponse);
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var serviceResponse = await _usersService.GetUserByIdAsync(userId);
                var mappedResponse = _mapper.Map<UserResponse>(serviceResponse);
                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null)
            {
                return errorResult;
            }

            try
            {
                var originalPassword = request.Password;
                request.Password = BC.HashPassword(request.Password);

                var mappedRequest = _mapper.Map<UserDto>(request);
                await _usersService.CreateUserAsync(mappedRequest);

                var (token, user) = await _accountsService.AuthenticateUserAsync(request.Email, originalPassword);

                return Ok(new { token, user = _mapper.Map<UserResponse>(user) });
            }
            catch (EmailAlreadyExistsException)
            {
                return BadRequest(new { error = "Email already exists" });
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest(new { error = "Failed to authenticate after registration" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;

            try
            {
                if (!string.IsNullOrEmpty(request.Password))
                {
                    request.Password = BC.HashPassword(request.Password);
                }

                var mappedRequest = _mapper.Map<UserDto>(request);
                var serviceResponse = await _usersService.UpdateUserAsync(userId, mappedRequest);
                var mappedResponse = _mapper.Map<UserResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _usersService.DeleteUserAsync(userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
