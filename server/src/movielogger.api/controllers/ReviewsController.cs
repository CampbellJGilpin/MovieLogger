using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.reviews;
using movielogger.api.models.responses.reviews;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;
        private readonly IMapper _mapper;

        public ReviewsController(IReviewsService reviewsService, IMapper mapper)
        {
            _reviewsService = reviewsService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("~/api/users/{userId}/reviews")]
        public async Task<IActionResult> GetUserReviews(int userId)
        {
            var serviceResponse = await _reviewsService.GetAllReviewsByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<List<ReviewResponse>>(serviceResponse);
            
            return Ok(mappedResponse);
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
            if (errorResult is not null) return errorResult;
            
            try 
            {
                var reviewRequest = _mapper.Map<ReviewDto>(request);
                var serviceResponse = await _reviewsService.CreateMovieReviewAsync(movieId, request.UserId, reviewRequest);
                var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);
                
                return CreatedAtAction(nameof(GetUserReviews), new { userId = request.UserId }, mappedResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("~/api/viewings/{viewingId}/review")]
        public async Task<IActionResult> CreateReview(int viewingId, [FromBody] CreateReviewRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;
            
            var reviewRequest = _mapper.Map<ReviewDto>(request);
            var serviceResponse = await _reviewsService.CreateReviewAsync(viewingId, reviewRequest);
            var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;
            
            var reviewRequest = _mapper.Map<ReviewDto>(request);
            var serviceResponse = await _reviewsService.UpdateReviewAsync(reviewId, reviewRequest);
            var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
    }
}
