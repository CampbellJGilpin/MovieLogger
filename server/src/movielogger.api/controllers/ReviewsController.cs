using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.reviews;
using movielogger.api.models.responses.reviews;
using movielogger.api.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    //[Authorize]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;
        private readonly IMapper _mapper;

        public ReviewsController(IReviewsService reviewsService, IMapper mapper)
        {
            _reviewsService = reviewsService;
            _mapper = mapper;
        }
        
        [HttpGet("users/{userId}/reviews")]
        public async Task<IActionResult> GetUserReviews(int userId)
        {
            var serviceResponse = await _reviewsService.GetAllReviewsByUserIdAsync(userId);
            var mappedResponse = _mapper.Map<List<ReviewResponse>>(serviceResponse);
            
            return Ok(mappedResponse);
        }
        
        [HttpPost("viewings/{viewingId}/reviews")]
        public async Task<IActionResult> CreateReview(int viewingId, [FromBody] CreateReviewRequest request)
        {
            var validator = new CreateReviewRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var reviewRequest = _mapper.Map<ReviewDto>(request);
            var serviceResponse = await _reviewsService.CreateReviewAsync(viewingId, reviewRequest);
            var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPut("reviews/{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewRequest request)
        {
            var validator = new UpdateReviewRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var reviewRequest = _mapper.Map<ReviewDto>(request);
            var serviceResponse = await _reviewsService.UpdateReviewAsync(reviewId, reviewRequest);
            var mappedResponse = _mapper.Map<ReviewResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
    }
}
