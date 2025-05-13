using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class ReviewsService : IReviewsService
{
    public Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ReviewDto> CreateReviewAsync(ReviewDto review)
    {
        throw new NotImplementedException();
    }

    public Task<ReviewDto> UpdateReviewAsync(int id, ReviewDto review)
    {
        throw new NotImplementedException();
    }
}