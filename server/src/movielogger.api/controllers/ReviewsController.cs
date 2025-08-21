using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.reviews;
using movielogger.api.models.responses.reviews;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController(IReviewsService reviewsService, IMapper mapper) : ControllerBase
    {
        private readonly IReviewsService _reviewsService = reviewsService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Route("~/api/users/{userId}/reviews")]
        public async Task<IActionResult> GetUserReviews(int userId)
        {
            try
            {
                var serviceResponse = await _reviewsService.GetAllReviewsByUserIdAsync(userId);
                var mappedResponse = _mapper.Map<List<ReviewResponse>>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {userId} not found.");
            }
        }

        [HttpGet]
        [Route("~/api/movies/{movieId}/users/{userId}/reviews")]
        public async Task<IActionResult> GetMovieReviewsByUser(int movieId, int userId)
        {
            try
            {
                var serviceResponse = await _reviewsService.GetMovieReviewsByUserIdAsync(movieId, userId);
                var mappedResponse = _mapper.Map<List<ReviewResponse>>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("~/api/movies/{movieId}/reviews")]
        public async Task<IActionResult> CreateMovieReview(int movieId, [FromBody] CreateReviewRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var reviewRequest = _mapper.Map<ReviewDto>(request);
                var serviceResponse = await _reviewsService.CreateMovieReviewAsync(movieId, request.UserId, reviewRequest);
                if (serviceResponse == null)
                    return BadRequest("Invalid movie or user ID");
                var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);

                return CreatedAtAction(nameof(GetUserReviews), new { userId = request.UserId }, mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("~/api/viewings/{viewingId}/reviews")]
        public async Task<IActionResult> CreateReview(int viewingId, [FromBody] CreateReviewRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var reviewRequest = _mapper.Map<ReviewDto>(request);
                var serviceResponse = await _reviewsService.CreateReviewAsync(viewingId, reviewRequest);
                if (serviceResponse == null)
                    return NotFound("Invalid viewing ID");
                var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var reviewRequest = _mapper.Map<ReviewDto>(request);
                var serviceResponse = await _reviewsService.UpdateReviewAsync(reviewId, reviewRequest);
                if (serviceResponse == null)
                    return NotFound($"Review with ID {reviewId} not found.");
                var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Review with ID {reviewId} not found.");
            }
        }

        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            try
            {
                var serviceResponse = await _reviewsService.GetReviewByIdAsync(reviewId);
                if (serviceResponse == null)
                    return NotFound($"Review with ID {reviewId} not found.");
                var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Review with ID {reviewId} not found.");
            }
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                await _reviewsService.DeleteReviewAsync(reviewId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Review with ID {reviewId} not found.");
            }
        }
    }
}
