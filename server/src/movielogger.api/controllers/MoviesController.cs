using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.movies;
using movielogger.api.models.responses;
using movielogger.api.models.responses.movies;
using movielogger.dal.dtos;
using movielogger.messaging.Models;
using movielogger.messaging.Services;
using movielogger.services.interfaces;
using movielogger.api.validation;

namespace movielogger.api.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieQueryService _movieQueryService;
        private readonly IMovieCommandService _movieCommandService;
        private readonly IMapper _mapper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IFileUploadService _fileUploadService;

        public MoviesController(
            IMovieQueryService movieQueryService,
            IMovieCommandService movieCommandService,
            IMapper mapper, 
            IMessagePublisher messagePublisher,
            IFileUploadService fileUploadService)
        {
            _movieQueryService = movieQueryService;
            _movieCommandService = movieCommandService;
            _mapper = mapper;
            _messagePublisher = messagePublisher;
            _fileUploadService = fileUploadService;
        }
        
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<MovieResponse>>> GetAllMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (movies, totalCount) = await _movieQueryService.GetAllMoviesAsync(page, pageSize);
            
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
                var movie = await _movieQueryService.GetMovieByIdAsync(id);
                return Ok(_mapper.Map<MovieResponse>(movie));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<MovieResponse>> CreateMovie([FromForm] CreateMovieRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var movieDto = _mapper.Map<MovieDto>(request);

                // Handle poster upload
                if (request.Poster != null && request.Poster.Length > 0)
                {
                    var posterPath = await _fileUploadService.UploadPosterAsync(request.Poster);
                    movieDto.PosterPath = posterPath;
                }

                var createdMovie = await _movieCommandService.CreateMovieAsync(movieDto);
                
                // Publish MovieAddedEvent
                var movieAddedEvent = new MovieAddedEvent
                {
                    MovieId = createdMovie.Id,
                    MovieTitle = createdMovie.Title,
                    Genre = createdMovie.Genre?.Title ?? "Unknown",
                    UserId = User.Identity?.Name ?? "Unknown"
                };
                
                await _messagePublisher.PublishAsync(movieAddedEvent);
                
                return CreatedAtAction(nameof(GetMovieById), new { id = createdMovie.Id }, _mapper.Map<MovieResponse>(createdMovie));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MovieResponse>> UpdateMovie(int id, [FromForm] UpdateMovieRequest request)
        {
            try
            {
                var movieDto = _mapper.Map<MovieDto>(request);

                // Handle poster upload
                if (request.Poster != null && request.Poster.Length > 0)
                {
                    var posterPath = await _fileUploadService.UploadPosterAsync(request.Poster);
                    movieDto.PosterPath = posterPath;
                }

                var updatedMovie = await _movieCommandService.UpdateMovieAsync(id, movieDto);
                
                // Publish MovieUpdatedEvent
                var movieUpdatedEvent = new MovieUpdatedEvent
                {
                    MovieId = updatedMovie.Id,
                    MovieTitle = updatedMovie.Title,
                    Genre = updatedMovie.Genre?.Title ?? "Unknown",
                    ChangedFields = GetChangedFields(request), // You'd need to implement this
                    UserId = User.Identity?.Name ?? "Unknown"
                };
                
                await _messagePublisher.PublishAsync(movieUpdatedEvent);
                
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
                // Get movie details before deletion for the event
                var movie = await _movieQueryService.GetMovieByIdAsync(id);
                
                var deleted = await _movieCommandService.DeleteMovieAsync(id);
                
                if (!deleted)
                {
                    return NotFound();
                }
                
                // Publish MovieDeletedEvent
                var movieDeletedEvent = new MovieDeletedEvent
                {
                    MovieId = movie.Id,
                    MovieTitle = movie.Title,
                    UserId = User.Identity?.Name ?? "Unknown"
                };
                
                await _messagePublisher.PublishAsync(movieDeletedEvent);
                
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
                var movies = await _movieQueryService.SearchMoviesAsync(q);
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
            var movies = await _movieQueryService.GetAllMoviesForUserAsync(userId, q, page, pageSize);
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

        [HttpPost("clear-cache")]
        public async Task<ActionResult> ClearCache()
        {
            // Note: Cache clearing is now handled by the CachedMoviesService
            // This endpoint can be removed or modified to work with the new architecture
            return Ok(new { message = "Cache clearing is handled automatically by the caching service" });
        }

        private string[] GetChangedFields(UpdateMovieRequest request)
        {
            var changedFields = new List<string>();
            
            if (!string.IsNullOrEmpty(request.Title))
                changedFields.Add("Title");
            if (!string.IsNullOrEmpty(request.Description))
                changedFields.Add("Description");
            if (request.ReleaseDate.HasValue)
                changedFields.Add("ReleaseDate");
            if (request.GenreId.HasValue)
                changedFields.Add("GenreId");
                
            return changedFields.ToArray();
        }
    }
}
