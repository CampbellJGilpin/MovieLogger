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
        
        public ViewingsController(IViewingsService viewingsService)
        {
            _viewingsService = viewingsService;
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
