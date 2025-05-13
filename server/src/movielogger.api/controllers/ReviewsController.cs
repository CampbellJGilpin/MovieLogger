using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;
        
        public ReviewsController(IReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }
        
        [HttpGet("Users/{userId}/Reviews")]
        public IActionResult GetUserReviews(int userId)
        {
            return Ok();
        }
        
        [HttpPost("Viewings/{viewingId}/Reviews")]
        public IActionResult CreateReview(int viewingId, [FromBody] AddReviewRequest request)
        {
            return Ok();
        }

        [HttpPut("Reviews/{reviewId}")]
        public IActionResult UpdateReview(int reviewId, [FromBody] AddReviewRequest request)
        {
            return Ok();
        }
    }
}
