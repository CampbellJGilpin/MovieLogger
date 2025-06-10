using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IReviewsService
{
    Task<IEnumerable<ReviewDto>> GetAllReviewsByUserIdAsync(int userId);
    Task<ReviewDto> CreateReviewAsync(int viewingId, ReviewDto review);
    Task<ReviewDto> CreateMovieReviewAsync(int movieId, int userId, ReviewDto review);
    Task<List<ReviewDto>> GetMovieReviewsByUserIdAsync(int movieId, int userId);
    Task<ReviewDto> UpdateReviewAsync(int reviewId, ReviewDto review);
}