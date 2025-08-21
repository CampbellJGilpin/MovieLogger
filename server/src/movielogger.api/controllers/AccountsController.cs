using Microsoft.AspNetCore.Mvc;
using movielogger.services.interfaces;
using AutoMapper;
using movielogger.api.models.requests.users;
using movielogger.api.models.responses.users;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using movielogger.api.validation;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController(IAccountsService accountsService, IUsersService usersService, IMapper mapper) : ControllerBase
    {
        private readonly IAccountsService _accountsService = accountsService;
        private readonly IUsersService _usersService = usersService;
        private readonly IMapper _mapper = mapper;

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var user = await _accountsService.Register(request.Email, request.Password, request.UserName);
                var (_, token) = await _accountsService.AuthenticateUserAsync(request.Email, request.Password);
                return Ok(new LoginResponse
                {
                    User = _mapper.Map<UserResponse>(user),
                    Token = token
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (user, token) = await _accountsService.AuthenticateUserAsync(request.Email, request.Password);
                return Ok(new LoginResponse
                {
                    User = _mapper.Map<UserResponse>(user),
                    Token = token
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _usersService.GetUserByIdAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserResponse>(user));
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {

            var errorResult = request.Validate();
            if (errorResult is not null)
            {
                return errorResult;
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                await _accountsService.ChangePasswordAsync(int.Parse(userId), request.CurrentPassword, request.NewPassword);
                return Ok(new { message = "Password changed successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
