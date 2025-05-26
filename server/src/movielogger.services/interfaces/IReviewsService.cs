using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IReviewsService
{
    Task<IEnumerable<ReviewDto>> GetAllReviewsByUserIdAsync(int userId);
    Task<ReviewDto> CreateReviewAsync(int viewingId, ReviewDto review);
    Task<ReviewDto> UpdateReviewAsync(int reviewId, ReviewDto review);
}