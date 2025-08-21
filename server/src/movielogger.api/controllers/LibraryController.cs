using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.library;
using movielogger.api.models.responses;
using movielogger.api.models.responses.library;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class LibraryController(ILibraryService libraryService, IMapper mapper) : ControllerBase
    {
        private readonly ILibraryService _libraryService = libraryService;
        private readonly IMapper _mapper = mapper;

        [HttpGet("{userId}/library")]
        public async Task<IActionResult> GetLibrary(int userId)
        {
            try
            {
                var serviceResponse = await _libraryService.GetLibraryByUserIdAsync(userId);
                var mappedResponse = _mapper.Map<LibraryResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {userId} not found");
            }
        }

        [HttpGet("{userId}/library/{movieId}")]
        public async Task<IActionResult> GetLibraryItem(int userId, int movieId)
        {
            try
            {
                var serviceResponse = await _libraryService.GetLibraryItemAsync(userId, movieId);
                if (serviceResponse == null)
                {
                    return NotFound($"Library item not found for user {userId} and movie {movieId}");
                }

                var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {userId} not found");
            }
        }

        [HttpPost("{userId}/library")]
        public async Task<IActionResult> AddToLibrary(int userId, [FromBody] CreateLibraryItemRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var mappedRequest = _mapper.Map<LibraryItemDto>(request);
                var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
                if (serviceResponse == null)
                    return BadRequest("Invalid user or movie ID");
                var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                // Check if it's a movie not found vs user not found
                if (ex.Message.Contains("Movie"))
                {
                    return BadRequest("Invalid movie ID");
                }
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{userId}/library/{movieId}")]
        public async Task<IActionResult> UpdateLibraryEntry(int userId, int movieId, [FromBody] UpdateLibraryItemRequest request)
        {
            // Set the MovieId from the route parameter
            request.MovieId = movieId;

            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var mappedRequest = _mapper.Map<LibraryItemDto>(request);
                var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
                if (serviceResponse == null)
                    return NotFound("Library item not found");
                var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{userId}/favorites/{movieId}")]
        public async Task<IActionResult> AddToFavorites(int userId, int movieId)
        {
            try
            {
                var request = new UpdateLibraryItemRequest
                {
                    MovieId = movieId,
                    IsFavorite = true,
                    OwnsMovie = true
                };

                var mappedRequest = _mapper.Map<LibraryItemDto>(request);
                var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
                if (serviceResponse == null)
                    return BadRequest("Invalid user or movie ID");
                var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{userId}/favorites/{movieId}")]
        public async Task<IActionResult> RemoveFromFavorites(int userId, int movieId)
        {
            try
            {
                var request = new UpdateLibraryItemRequest
                {
                    MovieId = movieId,
                    IsFavorite = false,
                    OwnsMovie = true
                };

                var mappedRequest = _mapper.Map<LibraryItemDto>(request);
                var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
                if (serviceResponse == null)
                    return NotFound("Library item not found");
                var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{userId}/library/{movieId}")]
        public async Task<IActionResult> AddToLibrary(int userId, int movieId)
        {
            try
            {
                var request = new UpdateLibraryItemRequest
                {
                    MovieId = movieId,
                    IsFavorite = false,
                    OwnsMovie = true
                };

                var mappedRequest = _mapper.Map<LibraryItemDto>(request);
                var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
                if (serviceResponse == null)
                    return BadRequest("Invalid user or movie ID");
                var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{userId}/library/{movieId}")]
        public async Task<IActionResult> RemoveFromLibrary(int userId, int movieId)
        {
            try
            {
                var removed = await _libraryService.RemoveFromLibraryAsync(userId, movieId);
                if (!removed)
                {
                    return NotFound($"Library item not found for user {userId} and movie {movieId}");
                }

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{userId}/library/paginated")]
        public async Task<IActionResult> GetLibraryPaginated(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, totalCount, totalPages) = await _libraryService.GetLibraryByUserIdPaginatedAsync(userId, page, pageSize);

                var response = new PaginatedResponse<LibraryItemResponse>
                {
                    Items = _mapper.Map<IEnumerable<LibraryItemResponse>>(items),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };

                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {userId} not found");
            }
        }

        [HttpGet("{userId}/library/favourites/paginated")]
        public async Task<IActionResult> GetLibraryFavouritesPaginated(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, totalCount, totalPages) = await _libraryService.GetLibraryFavouritesByUserIdPaginatedAsync(userId, page, pageSize);

                var response = new PaginatedResponse<LibraryItemResponse>
                {
                    Items = _mapper.Map<IEnumerable<LibraryItemResponse>>(items),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };

                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {userId} not found");
            }
        }
    }
}
