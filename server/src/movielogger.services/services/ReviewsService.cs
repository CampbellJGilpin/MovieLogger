using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class ReviewsService : IReviewsService
{
    private readonly AssessmentDbContext _db;
    private readonly IMapper _mapper;

    public ReviewsService(AssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllReviewsByUserIdAsync(int userId)
    {
        var reviews = await _db.Reviews
            .Include(r => r.Viewing)
            .ThenInclude(v => v.UserMovie)
            .Where(r => r.Viewing.UserMovie.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto> CreateReviewAsync(ReviewDto dto)
    {
        var review = _mapper.Map<Review>(dto);
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto> UpdateReviewAsync(int id, ReviewDto dto)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null)
        {
            throw new KeyNotFoundException($"Review with ID {id} not found.");
        }

        _mapper.Map(dto, review);
        await _db.SaveChangesAsync();

        return _mapper.Map<ReviewDto>(review);
    }
}