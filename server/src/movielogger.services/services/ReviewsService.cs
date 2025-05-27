using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class ReviewsService : IReviewsService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public ReviewsService(IAssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllReviewsByUserIdAsync(int userId)
    {
        var reviews = await _db.Reviews
            .Include(r => r.Viewing)
            .ThenInclude(v => v.UserMovie)
            .ThenInclude(x => x.Movie)
            .Where(r => r.Viewing.UserMovie.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto> CreateReviewAsync(int viewingId, ReviewDto dto)
    {
        var review = _mapper.Map<Review>(dto);
        review.ViewingId = viewingId;
        
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto> UpdateReviewAsync(int reviewId, ReviewDto dto)
    {
        var review = await _db.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            throw new KeyNotFoundException($"Review with ID {reviewId} not found.");
        }
        
        _mapper.Map(dto, review);
        await _db.SaveChangesAsync();

        var savedReview = await _db.Reviews
            .Include(r => r.Viewing)
            .ThenInclude(v => v.UserMovie)
            .ThenInclude(x => x.Movie)
            .FirstOrDefaultAsync(v => v.Id == reviewId);
        
        return _mapper.Map<ReviewDto>(savedReview);
    }
}