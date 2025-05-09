using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;

namespace movielogger.api.controllers
{
    [Route("users/{userId}/library")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLibrary(int userId)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult AddToLibrary(int userId, [FromBody] AddLibraryRequest request)
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateLibraryEntry(int userId, [FromBody] UpdateLibraryRequest request)
        {
            return Ok();
        }
        
        [HttpGet("favourites")]
        public IActionResult GetFavourites(int id)
        {
            return Ok();
        }

        [HttpGet("watchlist")]
        public IActionResult GetWatchlist(int id)
        {
            return Ok();
        }
    }
}
