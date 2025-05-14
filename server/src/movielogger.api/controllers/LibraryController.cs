using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;
        private readonly IMapper _mapper;

        public LibraryController(ILibraryService libraryService, IMapper mapper)
        {
            _libraryService = libraryService;
            _mapper = mapper;
        }
        
        [HttpGet("Users/{userId}/Library")]
        public IActionResult GetLibrary(int userId)
        {
            return Ok();
        }

        [HttpPost("Users/{userId}/Library")]
        public IActionResult AddToLibrary(int userId, [FromBody] CreateLibraryRequest request)
        {
            return Ok();
        }

        [HttpPut("Users/{userId}/Library")]
        public IActionResult UpdateLibraryEntry(int userId, [FromBody] UpdateLibraryRequest request)
        {
            return Ok();
        }
        
        [HttpGet("Users/{userId}/Library/Favourites")]
        public IActionResult GetFavourites(int id)
        {
            return Ok();
        }
        
        [HttpGet("Users/{userId}/Library/Watchlist")]
        public IActionResult GetWatchlist(int id)
        {
            return Ok();
        }
    }
}
