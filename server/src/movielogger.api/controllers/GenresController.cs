using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.dal.models;

namespace movielogger.api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllGenres()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetGenreById(int id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateGenre([FromBody] AddGenreRequest request)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateGenre(int id, [FromBody] UpdateGenreRequest request)
        {
            return Ok();
        }
    }
}
