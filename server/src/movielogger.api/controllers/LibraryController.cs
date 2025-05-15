using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.api.models.requests.library;
using movielogger.api.models.responses.library;
using movielogger.api.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
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
        
        [HttpGet("Users/{userId}/Library")]
        public async Task<IActionResult> GetLibrary(int userId)
        {
            var serviceResponse = await _libraryService.GetLibraryByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<LibraryResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPost("Users/{userId}/Library")]
        public async Task<IActionResult> AddToLibrary(int userId, [FromBody] CreateLibraryRequest request)
        {
            var validator = new CreateLibraryRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.CreateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPut("Users/{userId}/Library")]
        public async Task<IActionResult> UpdateLibraryEntry(int userId, [FromBody] UpdateLibraryRequest request)
        {
            var validator = new UpdateLibraryRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var mappedRequest = _mapper.Map<LibraryItemDto>(request);
            var serviceResponse = await _libraryService.UpdateLibraryEntryAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<LibraryItemResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpGet("Users/{userId}/Library/Favourites")]
        public async Task<IActionResult> GetFavourites(int userId)
        {
            var serviceResponse = await _libraryService.GetLibraryFavouritesByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<LibraryResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpGet("Users/{userId}/Library/Watchlist")]
        public async Task<IActionResult> GetWatchlist(int userId)
        {
            var serviceResponse = await _libraryService.GetLibraryWatchlistByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<LibraryResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
    }
}
