using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("Viewings")]
    public class ViewingsController : ControllerBase
    {
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
