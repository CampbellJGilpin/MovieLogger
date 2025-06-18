using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.movies;
using movielogger.api.models.responses;
using movielogger.api.models.responses.movies;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
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
        public async Task<ActionResult<PaginatedResponse<MovieResponse>>> GetAllMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (movies, totalCount) = await _moviesService.GetAllMoviesAsync(page, pageSize);
            
            var response = new PaginatedResponse<MovieResponse>
            {
                Items = _mapper.Map<IEnumerable<MovieResponse>>(movies),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
            
            return Ok(response);
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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MovieResponse>>> SearchMovies([FromQuery] string q)
        {
            try
            {
                var movies = await _moviesService.SearchMoviesAsync(q);
                return Ok(_mapper.Map<IEnumerable<MovieResponse>>(movies));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("all-for-user")]
        public async Task<ActionResult<PaginatedResponse<UserMovieResponse>>> GetAllMoviesForUser(
            [FromQuery] int userId,
            [FromQuery] string? q = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var movies = await _moviesService.GetAllMoviesForUserAsync(userId, q, page, pageSize);
            var response = new PaginatedResponse<UserMovieResponse>
            {
                Items = _mapper.Map<IEnumerable<UserMovieResponse>>(movies.Items),
                Page = page,
                PageSize = pageSize,
                TotalCount = movies.TotalCount,
                TotalPages = movies.TotalPages
            };
            return Ok(response);
        }
    }
}
