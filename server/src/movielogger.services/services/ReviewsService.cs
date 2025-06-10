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

    public async Task<ReviewDto> CreateMovieReviewAsync(int movieId, int userId, ReviewDto review)
    {
        // Check if movie exists
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {movieId} not found.");
        }

        // Get or create UserMovie
        var userMovie = await _db.UserMovies
            .FirstOrDefaultAsync(um => um.MovieId == movieId && um.UserId == userId);

        if (userMovie == null)
        {
            userMovie = new UserMovie
            {
                UserId = userId,
                MovieId = movieId,
                Favourite = false,
                OwnsMovie = false,
                UpcomingViewDate = null
            };
            _db.UserMovies.Add(userMovie);
            await _db.SaveChangesAsync();
        }

        // Create a viewing
        var viewing = new Viewing 
        { 
            UserMovieId = userMovie.Id,
            DateViewed = DateTime.UtcNow
        };
        _db.Viewings.Add(viewing);
        await _db.SaveChangesAsync();

        // Create the review
        var reviewEntity = _mapper.Map<Review>(review);
        reviewEntity.ViewingId = viewing.Id;
        
        _db.Reviews.Add(reviewEntity);
        await _db.SaveChangesAsync();

        // Load the complete review with all relationships
        var savedReview = await _db.Reviews
            .Include(r => r.Viewing)
                .ThenInclude(v => v.UserMovie)
                    .ThenInclude(um => um.Movie)
            .FirstAsync(r => r.Id == reviewEntity.Id);

        return _mapper.Map<ReviewDto>(savedReview);
    }

    public async Task<List<ReviewDto>> GetMovieReviewsByUserIdAsync(int movieId, int userId)
    {
        // First check if movie exists
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {movieId} not found.");
        }

        var reviews = await _db.Reviews
            .Include(r => r.Viewing)
                .ThenInclude(v => v.UserMovie)
                    .ThenInclude(um => um.Movie)
            .Where(r => r.Viewing.UserMovie.MovieId == movieId && r.Viewing.UserMovie.UserId == userId)
            .OrderByDescending(r => r.Viewing.DateViewed)
            .ToListAsync();

        return _mapper.Map<List<ReviewDto>>(reviews);
    }
}