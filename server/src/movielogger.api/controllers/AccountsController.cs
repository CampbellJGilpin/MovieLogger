using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.services.interfaces;
using AutoMapper;
using movielogger.api.models.requests.users;

namespace movielogger.api.controllers
{
    //[Authorize]
    [Route("Accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountsService;
        private readonly IMapper _mapper;

        public AccountsController(IAccountsService accountsService, IMapper mapper)
        {
            _accountsService = accountsService;
            _mapper = mapper;
        }
        
        [HttpPost]
        public async Task <IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var response = await _accountsService.AuthenticateUserAsync(request.Email, request.Password);
            
            return Ok(response);
        }
    }
}
