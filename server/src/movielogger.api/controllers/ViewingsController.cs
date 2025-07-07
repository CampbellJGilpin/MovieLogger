using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.viewings;
using movielogger.api.models.responses.viewings;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/viewings")]
    public class ViewingsController : ControllerBase
    {
        private readonly IViewingsService _viewingsService;
        private readonly IMapper _mapper;

        public ViewingsController(IViewingsService viewingsService, IMapper mapper)
        {
            _viewingsService = viewingsService;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Route("~/api/users/{userId}/viewings")]
        public async Task<IActionResult> GetUserViewings(int userId)
        {
            try
            {
                var serviceResponse = await _viewingsService.GetViewingsByUserIdAsync(userId);
                var mappedResponse = _mapper.Map<List<ViewingResponse>>(serviceResponse);
                
                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {userId} not found.");
            }
        }
        
        [HttpGet("{viewingId}")]
        public async Task<IActionResult> GetViewing(int viewingId)
        {
            try
            {
                var serviceResponse = await _viewingsService.GetViewingByIdAsync(viewingId);
                if (serviceResponse == null)
                    return NotFound($"Viewing with ID {viewingId} not found.");
                var mappedResponse = _mapper.Map<ViewingResponse>(serviceResponse);
                
                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Viewing with ID {viewingId} not found.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateViewing([FromBody] CreateViewingRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);
            
            try
            {
                var viewingDto = _mapper.Map<ViewingDto>(request);
                var serviceResponse = await _viewingsService.CreateViewingAsync(request.UserId, viewingDto);
                if (serviceResponse == null)
                    return BadRequest("Invalid user or movie ID");
                var mappedResponse = _mapper.Map<ViewingResponse>(serviceResponse);
                if (mappedResponse == null)
                    return BadRequest("Invalid user or movie ID");
                return CreatedAtAction(nameof(GetViewing), new { viewingId = mappedResponse.ViewingId }, mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User or Movie not found");
            }
        }

        [HttpPost]
        [Route("~/api/users/{userId}/viewings")]
        public async Task<IActionResult> CreateUserViewing(int userId, [FromBody] CreateViewingRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);
            
            try
            {
                var viewingDto = _mapper.Map<ViewingDto>(request);
                var serviceResponse = await _viewingsService.CreateViewingAsync(userId, viewingDto);
                if (serviceResponse == null)
                    return BadRequest("Invalid user or movie ID");
                var mappedResponse = _mapper.Map<ViewingResponse>(serviceResponse);
                if (mappedResponse == null)
                    return BadRequest("Invalid user or movie ID");
                return CreatedAtAction(nameof(GetViewing), new { viewingId = mappedResponse.ViewingId }, mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User or Movie not found");
            }
        }
        
        [HttpPut("{viewingId}")]
        public async Task<IActionResult> UpdateViewing(int viewingId, [FromBody] UpdateViewingRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;

            try
            {
                var mappedRequest = _mapper.Map<ViewingDto>(request);
                var serviceResponse = await _viewingsService.UpdateViewingAsync(viewingId, mappedRequest);
                if (serviceResponse == null)
                    return NotFound($"Viewing with ID {viewingId} not found.");
                var mappedResponse = _mapper.Map<ViewingResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Viewing with ID {viewingId} not found.");
            }
        }

        [HttpDelete("{viewingId}")]
        public async Task<IActionResult> DeleteViewing(int viewingId)
        {
            try
            {
                await _viewingsService.DeleteViewingAsync(viewingId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Viewing with ID {viewingId} not found.");
            }
        }
    }
}
