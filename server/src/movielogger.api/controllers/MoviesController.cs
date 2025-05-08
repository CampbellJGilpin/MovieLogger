using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.dal.models;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllMovies()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetMovieById(int id)
        {
            return Ok();
        }
        
        [HttpPost]
        public IActionResult AddMovie([FromBody] AddMovieRequest request)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMovie(int id, [FromBody] UpdateMovieRequest request)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            return Ok();
        }
    }
}
