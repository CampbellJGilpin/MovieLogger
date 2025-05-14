using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.services.interfaces;
using AutoMapper;

namespace movielogger.api.controllers
{
    [ApiController]
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
        public IActionResult Login([FromBody] LoginUserRequest request)
        {
            return Ok();
        }
    }
}
