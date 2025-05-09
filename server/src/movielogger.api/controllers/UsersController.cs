using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok();
        }

        [HttpGet("{userId}")]
        public IActionResult GetUserById(int userId)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult RegisterUser([FromBody] RegisterUserRequest request)
        {
            return Ok();
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            return Ok();
        }
    }
}
