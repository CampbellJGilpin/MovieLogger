using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.library;
using movielogger.api.models.responses.library;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;
        private readonly IMapper _mapper;

        public LibraryController(ILibraryService libraryService, IMapper mapper)
        {
            _libraryService = libraryService;
            _mapper = mapper;
        }
        
        [HttpGet("{userId}/library")]
        public async Task<IActionResult> GetLibrary(int userId)
        {
            var serviceResponse = await _libraryService.GetLibraryByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<LibraryResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPost("{userId}/library")]
        public async Task<IActionResult> AddToLibrary(int userId, [FromBody] CreateLibraryItemRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;

            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPut("{userId}/library")]
        public async Task<IActionResult> UpdateLibraryEntry(int userId, [FromBody] UpdateLibraryItemRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;

            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPost("{userId}/favorites/{movieId}")]
        public async Task<IActionResult> AddToFavorites(int userId, int movieId)
        {
            var request = new UpdateLibraryItemRequest
            {
                MovieId = movieId,
                IsFavorite = true,
                OwnsMovie = true
            };

            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpDelete("{userId}/favorites/{movieId}")]
        public async Task<IActionResult> RemoveFromFavorites(int userId, int movieId)
        {
            var request = new UpdateLibraryItemRequest
            {
                MovieId = movieId,
                IsFavorite = false,
                OwnsMovie = true
            };

            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPost("{userId}/library/{movieId}")]
        public async Task<IActionResult> AddToLibrary(int userId, int movieId)
        {
            var request = new UpdateLibraryItemRequest
            {
                MovieId = movieId,
                IsFavorite = false,
                OwnsMovie = true
            };

            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpDelete("{userId}/library/{movieId}")]
        public async Task<IActionResult> RemoveFromLibrary(int userId, int movieId)
        {
            var existingItem = await _libraryService.GetLibraryItemAsync(userId, movieId);
            if (existingItem == null)
            {
                return NoContent();
            }

            var request = new UpdateLibraryItemRequest
            {
                MovieId = movieId,
                IsFavorite = existingItem.Favourite,
                OwnsMovie = false
            };

            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
    }
}
