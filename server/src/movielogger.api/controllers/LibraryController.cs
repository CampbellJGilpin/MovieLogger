using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Route("Users/{userId}/Library")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService libraryService;
        
        public LibraryController(ILibraryService libraryService)
        {
            this.libraryService = libraryService;
        }
        
        [HttpGet]
        public IActionResult GetLibrary(int userId)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult AddToLibrary(int userId, [FromBody] CreateLibraryRequest request)
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateLibraryEntry(int userId, [FromBody] UpdateLibraryRequest request)
        {
            return Ok();
        }
        
        [HttpGet("Favourites")]
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
