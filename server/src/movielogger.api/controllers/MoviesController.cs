using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("movies")]
    public class MoviesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllMovies()
        {
            return Ok();
        }

        [HttpGet("{movieId}")]
        public IActionResult GetMovieById(int movieId)
        {
            return Ok();
        }
        
        [HttpPost]
        public IActionResult AddMovie([FromBody] AddMovieRequest request)
        {
            return Ok();
        }

        [HttpPut("{movieId}")]
        public IActionResult UpdateMovie(int movieId, [FromBody] UpdateMovieRequest request)
        {
            return Ok();
        }

        [HttpDelete("{movieId}")]
        public IActionResult DeleteMovie(int movieId)
        {
            return Ok();
        }
    }
}
