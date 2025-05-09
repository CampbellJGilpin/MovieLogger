using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("reviews")]
    public class ReviewsController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateReview(int viewingId, [FromBody] AddReviewRequest request)
        {
            return Ok();
        }

        [HttpPut("{reviewId}")]
        public IActionResult UpdateReview(int viewingId, int reviewId, [FromBody] AddReviewRequest request)
        {
            return Ok();
        }
    }
}
