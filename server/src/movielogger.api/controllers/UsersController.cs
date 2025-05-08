using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult RegisterUser([FromBody] RegisterUserRequest request){
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] RegisterUserRequest request)
        {
            return Ok();
        }

        [HttpGet("{id}/library")]
        public IActionResult GetUserLibrary(int id)
        {
            return Ok();
        }

        [HttpGet("{id}/favourites")]
        public IActionResult GetFavourites(int id)
        {
            return Ok();
        }

        [HttpGet("{id}/watchlist")]
        public IActionResult GetWatchlist(int id)
        {
            return Ok();
        }
    }
}
