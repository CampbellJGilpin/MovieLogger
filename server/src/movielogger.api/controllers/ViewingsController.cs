using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.viewings;
using movielogger.api.models.responses.viewings;
using movielogger.api.validation;
using movielogger.api.validation.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    //[Authorize]
    [ApiController]
    [Route("viewings")]
    public class ViewingsController : ControllerBase
    {
        private readonly IViewingsService _viewingsService;
        private readonly IMapper _mapper;

        public ViewingsController(IViewingsService viewingsService, IMapper mapper)
        {
            _viewingsService = viewingsService;
            _mapper = mapper;
        }
        
        [HttpGet("{viewingId}")]
        public async Task<IActionResult> GetViewing(int viewingId)
        {
            try
            {
                var serviceResponse = await _viewingsService.GetViewingByIdAsync(viewingId);
                var mappedResponse = _mapper.Map<ViewingReponse>(serviceResponse);
                
                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"Viewing with ID {viewingId} not found.");
            }
        }
        
        [HttpPost("users/{userId}/viewings")]
        public async Task<IActionResult> CreateViewing(int userId, [FromBody] CreateViewingRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;
            
            var viewingDto = _mapper.Map<ViewingDto>(request);
            var serviceResponse = await _viewingsService.CreateViewingAsync(userId, viewingDto);
            var mappedResponse = _mapper.Map<ViewingReponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpPut("viewings/{viewingId}")]
        public async Task<IActionResult> UpdateViewing(int viewingId, [FromBody] UpdateViewingRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;

            var mappedRequest = _mapper.Map<ViewingDto>(request);
            var serviceResponse = await _viewingsService.UpdateViewingAsync(viewingId, mappedRequest);
            var mappedResponse = _mapper.Map<ViewingReponse>(serviceResponse);

            return Ok(mappedResponse);
        }
        
    }
}
