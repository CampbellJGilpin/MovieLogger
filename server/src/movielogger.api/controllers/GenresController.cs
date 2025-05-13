using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Route("Genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;
        
        public GenresController(IGenresService genresService)
        {
            _genresService = genresService;
        }
        
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
        public IActionResult CreateGenre([FromBody] CreateGenreRequest request)
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
