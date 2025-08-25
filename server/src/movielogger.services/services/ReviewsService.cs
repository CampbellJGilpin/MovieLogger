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
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var reviews = await _db.Reviews
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.User)
            .Where(r => r.UserMovieViewing.UserId == userId)
            .OrderByDescending(r => r.UserMovieViewing.DateViewed)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto> CreateReviewAsync(int userMovieViewingId, ReviewDto dto)
    {
        // Check if user movie viewing exists
        var viewing = await _db.UserMovieViewings
            .Include(v => v.Movie)
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Id == userMovieViewingId);

        if (viewing == null)
        {
            throw new KeyNotFoundException($"User movie viewing with ID {userMovieViewingId} not found.");
        }

        // Check if review already exists for this viewing
        var existingReview = await _db.Reviews.FirstOrDefaultAsync(r => r.UserMovieViewingId == userMovieViewingId);
        if (existingReview != null)
        {
            throw new InvalidOperationException($"A review already exists for viewing with ID {userMovieViewingId}.");
        }

        var review = _mapper.Map<Review>(dto);
        review.UserMovieViewingId = userMovieViewingId;

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        // Reload with includes for the response
        var createdReview = await _db.Reviews
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.User)
            .FirstOrDefaultAsync(r => r.Id == review.Id);

        return _mapper.Map<ReviewDto>(createdReview!);
    }

    public async Task<ReviewDto> CreateMovieReviewAsync(int movieId, int userId, ReviewDto reviewDto)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        // Check if movie exists
        var movieExists = await _db.Movies.AnyAsync(m => m.Id == movieId && !m.IsDeleted);
        if (!movieExists)
        {
            throw new KeyNotFoundException($"Movie with ID {movieId} not found.");
        }

        // Create new viewing record with current date
        var viewing = new UserMovieViewing
        {
            UserId = userId,
            MovieId = movieId,
            DateViewed = DateTime.UtcNow
        };

        _db.UserMovieViewings.Add(viewing);
        await _db.SaveChangesAsync();

        // Create the review
        var review = _mapper.Map<Review>(reviewDto);
        review.UserMovieViewingId = viewing.Id;

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        // Return the created review with full data
        var createdReview = await _db.Reviews
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.User)
            .FirstOrDefaultAsync(r => r.Id == review.Id);

        return _mapper.Map<ReviewDto>(createdReview!);
    }

    public async Task<List<ReviewDto>> GetMovieReviewsByUserIdAsync(int movieId, int userId)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        // Check if movie exists
        var movieExists = await _db.Movies.AnyAsync(m => m.Id == movieId && !m.IsDeleted);
        if (!movieExists)
        {
            throw new KeyNotFoundException($"Movie with ID {movieId} not found.");
        }

        var reviews = await _db.Reviews
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.User)
            .Where(r => r.UserMovieViewing.MovieId == movieId && r.UserMovieViewing.UserId == userId)
            .OrderByDescending(r => r.UserMovieViewing.DateViewed)
            .ToListAsync();

        return _mapper.Map<List<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto> UpdateReviewAsync(int reviewId, ReviewDto dto)
    {
        var review = await _db.Reviews
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.User)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review == null)
        {
            throw new KeyNotFoundException($"Review with ID {reviewId} not found.");
        }

        review.ReviewText = dto.ReviewText;
        review.Score = dto.Score;

        await _db.SaveChangesAsync();

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto> GetReviewByIdAsync(int reviewId)
    {
        var review = await _db.Reviews
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(r => r.UserMovieViewing)
            .ThenInclude(v => v.User)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review == null)
        {
            throw new KeyNotFoundException($"Review with ID {reviewId} not found.");
        }

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        var review = await _db.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            throw new KeyNotFoundException($"Review with ID {reviewId} not found.");
        }

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();
    }
}