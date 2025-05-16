using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.api.models.requests.reviews;
using movielogger.api.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
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
        
        [HttpGet("Users/{userId}/Reviews")]
        public async Task<IActionResult> GetUserReviews(int userId)
        {
            var serviceResponse = await _reviewsService.GetAllReviewsByUserIdAsync(userId);
            
            return Ok();
        }
        
        [HttpPost("Viewings/{viewingId}/Reviews")]
        public async Task<IActionResult> CreateReview(int viewingId, [FromBody] CreateReviewRequest request)
        {
            var validator = new CreateReviewRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var reviewRequest = _mapper.Map<ReviewDto>(request);
            var serviceResponse = await _reviewsService.CreateReviewAsync(reviewRequest);
            var mappedResponse = _mapper.Map<ReviewDto>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPut("Reviews/{reviewId}")]
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
            var mappedResponse = _mapper.Map<ReviewDto>(serviceResponse);
            
            return Ok(mappedResponse);
        }
    }
}
