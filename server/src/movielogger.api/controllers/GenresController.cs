using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;

namespace movielogger.api.controllers
{
    [Route("genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllGenres()
        {
            return Ok();
        }

        [HttpGet("{genreId}")]
        public IActionResult GetGenreById(int genreId)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateGenre([FromBody] AddGenreRequest request)
        {
            return Ok();
        }

        [HttpPut("{genreId}")]
        public IActionResult UpdateGenre(int genreId, [FromBody] UpdateGenreRequest request)
        {
            return Ok();
        }
    }
}
