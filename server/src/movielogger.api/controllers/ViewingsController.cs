using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("viewings")]
    public class ViewingsController : ControllerBase
    {
        [HttpGet("{viewingId}")]
        public IActionResult GetViewing(int viewingId)
        {
            return Ok();
        }
        
        [HttpPost]
        public IActionResult CreateViewing([FromBody] CreateViewingRequest request)
        {
            return Ok();
        }

        [HttpPost("{viewingId}/review")]
        public IActionResult CreateReview(int viewingId, [FromBody] CreateReviewRequest request)
        {
            return Ok();
        }
    }
}
