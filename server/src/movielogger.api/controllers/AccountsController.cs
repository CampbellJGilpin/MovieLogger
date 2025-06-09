using Microsoft.AspNetCore.Mvc;
using movielogger.services.interfaces;
using AutoMapper;
using movielogger.api.models.requests.users;
using movielogger.api.models.responses.users;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountsService;
        private readonly IMapper _mapper;

        public AccountsController(IAccountsService accountsService, IMapper mapper)
        {
            _accountsService = accountsService;
            _mapper = mapper;
        }
        
        [HttpPost("login")]
        public async Task <IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var (token, user) = await _accountsService.AuthenticateUserAsync(request.Email, request.Password);
            var mappedUser = _mapper.Map<UserResponse>(user);
            
            return Ok(new { token, user = mappedUser });
        }
    }
}
