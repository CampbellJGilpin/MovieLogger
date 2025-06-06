using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.users;
using movielogger.api.models.responses.users;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    //[Authorize]
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public UsersController(IUsersService usersService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var serviceResponse = await _usersService.GetAllUsersAsync();
            var mappedResponse = _mapper.Map<IList<UserResponse>>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpGet("{userId}")]
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
            if (errorResult is not null) return errorResult;
            
            try
            {
                var mappedRequest = _mapper.Map<UserDto>(request);
                var serviceResponse = await _usersService.CreateUserAsync(mappedRequest);
                var mappedResponse = _mapper.Map<UserResponse>(serviceResponse);
                
                return CreatedAtAction(nameof(GetUserById), new { userId = mappedResponse.Id }, mappedResponse);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email already exists"))
            {
                return BadRequest(new { error = "Email already exists" });
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;
            
            try
            {
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
