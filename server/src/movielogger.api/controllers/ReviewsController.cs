using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.dal.models;

namespace movielogger.api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
