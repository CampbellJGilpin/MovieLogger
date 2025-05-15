using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.api.models.requests.viewings;
using movielogger.api.models.responses.viewings;
using movielogger.api.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("Viewings")]
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
            var serviceResponse = await _viewingsService.GetViewingAsync(viewingId);
            var mappedResponse = _mapper.Map<ViewingDto>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpPost("Users/{userId}/Viewings")]
        public async Task<IActionResult> CreateViewing(int userId, [FromBody] CreateViewingRequest request)
        {
            var viewingValidator = new CreateViewingRequestValidator();
            var validationResult = await viewingValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var mappedRequest = _mapper.Map<ViewingDto>(request);
            var serviceResponse = _viewingsService.CreateViewingAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<ViewingReponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpPut("Users/{userId}/Viewings")]
        public async Task<IActionResult> UpdateViewing(int userId, [FromBody] UpdateViewingRequest request)
        {
            var viewingValidator = new UpdateViewingRequestValidator();
            var validationResult = await viewingValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var mappedRequest = _mapper.Map<ViewingDto>(request);
            var serviceResponse = _viewingsService.UpdateViewingAsync(userId, mappedRequest);
            var mappedResponse = _mapper.Map<ViewingReponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
    }
}
