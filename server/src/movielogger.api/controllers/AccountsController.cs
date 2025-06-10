using Microsoft.AspNetCore.Mvc;
using movielogger.services.interfaces;
using AutoMapper;
using movielogger.api.models.requests.users;
using movielogger.api.models.responses.users;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountsService;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public AccountsController(IAccountsService accountsService, IUsersService usersService, IMapper mapper)
        {
            _accountsService = accountsService;
            _usersService = usersService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
        {
            var user = await _accountsService.Register(request.Email, request.Password, request.UserName);
            return Ok(_mapper.Map<UserResponse>(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var (user, token) = await _accountsService.AuthenticateUserAsync(request.Email, request.Password);
            return Ok(new LoginResponse
            {
                User = _mapper.Map<UserResponse>(user),
                Token = token
            });
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
    }
}
