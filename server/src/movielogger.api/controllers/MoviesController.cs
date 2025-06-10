using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.movies;
using movielogger.api.models.responses.movies;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IMapper _mapper;

        public MoviesController(IMoviesService moviesService, IMapper mapper)
        {
            _moviesService = moviesService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieResponse>>> GetAllMovies()
        {
            var movies = await _moviesService.GetAllMoviesAsync();
            return Ok(_mapper.Map<IEnumerable<MovieResponse>>(movies));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieResponse>> GetMovieById(int id)
        {
            try
            {
                var movie = await _moviesService.GetMovieByIdAsync(id);
                return Ok(_mapper.Map<MovieResponse>(movie));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<MovieResponse>> CreateMovie([FromBody] CreateMovieRequest request)
        {
            try
            {
                var movieDto = _mapper.Map<MovieDto>(request);
                var createdMovie = await _moviesService.CreateMovieAsync(movieDto);
                return Ok(_mapper.Map<MovieResponse>(createdMovie));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MovieResponse>> UpdateMovie(int id, [FromBody] UpdateMovieRequest request)
        {
            try
            {
                var movieDto = _mapper.Map<MovieDto>(request);
                var updatedMovie = await _moviesService.UpdateMovieAsync(id, movieDto);
                return Ok(_mapper.Map<MovieResponse>(updatedMovie));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            try
            {
                await _moviesService.DeleteMovieAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
