using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
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
        public IActionResult GetViewing(int viewingId)
        {
            return Ok();
        }
        
        [HttpPost("Users/{userId}/Viewings")]
        public IActionResult CreateViewing([FromBody] CreateViewingRequest request)
        {
            return Ok();
        }
    }
}
