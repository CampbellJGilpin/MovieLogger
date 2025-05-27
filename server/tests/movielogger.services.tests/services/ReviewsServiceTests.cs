using movielogger.services.interfaces;
using movielogger.services.services;

namespace movielogger.services.tests.services;

public class ReviewsServiceTests : BaseServiceTest
{
    IReviewsService _service;
    
    public ReviewsServiceTests()
    {
        _service = new ReviewsService(_dbContext, _mapper);
    }
}