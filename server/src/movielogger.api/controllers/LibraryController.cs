using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.library;
using movielogger.api.models.responses.library;
using movielogger.api.validation;
using movielogger.api.validation.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    //[Authorize]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;
        private readonly IMapper _mapper;

        public LibraryController(ILibraryService libraryService, IMapper mapper)
        {
            _libraryService = libraryService;
            _mapper = mapper;
        }
        
        [HttpGet("users/{userId}/library")]
        public async Task<IActionResult> GetLibrary(int userId)
        {
            var serviceResponse = await _libraryService.GetLibraryByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<LibraryResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPost("users/{userId}/library")]
        public async Task<IActionResult> AddToLibrary(int userId, [FromBody] CreateLibraryItemRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;

            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.CreateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPut("users/{userId}/library")]
        public async Task<IActionResult> UpdateLibraryEntry(int userId, [FromBody] UpdateLibraryItemRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;
            
            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpGet("users/{userId}/library/favourites")]
        public async Task<IActionResult> GetFavourites(int userId)
        {
            var serviceResponse = await _libraryService.GetLibraryFavouritesByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<LibraryResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpGet("users/{userId}/library/watchlist")]
        public async Task<IActionResult> GetWatchlist(int userId)
        {
            var serviceResponse = await _libraryService.GetLibraryWatchlistByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<LibraryResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
    }
}
