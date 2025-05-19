using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.movies;
using movielogger.api.models.responses.movies;
using movielogger.api.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    //[Authorize]
    [ApiController]
    [Route("Movies")]
    public class MoviesController: ControllerBase
    {
        private readonly IMoviesService _movieService;
        private readonly IMapper _mapper;

        public MoviesController(IMoviesService movieService, IMapper mapper)
        {
            _movieService = movieService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var serviceResponse = await _movieService.GetAllMoviesAsync();
            var mappedResponse = _mapper.Map<List<MovieResponse>>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovieById(int movieId)
        {
            var serviceResponse = await _movieService.GetMovieByIdAsync(movieId);
            var mappedResponse = _mapper.Map<MovieResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequest request)
        {
            var validator = new CreateMovieRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var mappedRequest = _mapper.Map<MovieDto>(request);
            var serviceResponse = await _movieService.CreateMovieAsync(mappedRequest);
            var mappedResponse = _mapper.Map<MovieResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPut("{movieId}")]
        public async Task<IActionResult> UpdateMovie(int movieId, [FromBody] UpdateMovieRequest request)
        {
            var validator = new UpdateMovieRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var mappedRequest = _mapper.Map<MovieDto>(request);
            var serviceResponse = await _movieService.UpdateMovieAsync(movieId, mappedRequest);
            var mappedResponse = _mapper.Map<MovieResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpDelete("{movieId}")]
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            var serviceResponse = await _movieService.DeleteMovieAsync(movieId);
            
            return Ok(serviceResponse);
        }
    }
}
