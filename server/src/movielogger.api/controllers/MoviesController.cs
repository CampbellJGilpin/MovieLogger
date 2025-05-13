using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.api.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _movieService;
        
        public MoviesController(IMoviesService movieService)
        {
            _movieService = movieService;
        }
        
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
        public IActionResult AddMovie([FromBody] CreateMovieRequest request)
        {
            var validator = new CreateMovieRequestValidator();
            var results = validator.Validate(request);

            if (!results.IsValid)
            {
                
            }

            var movieDto = new MovieDto(); // Automapper to turn request to MovieDto
            _movieService.CreateMovieAsync(movieDto);
            
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
