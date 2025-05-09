using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] UserLoginRequest request)
        {
            return Ok();
        }
    }
}
