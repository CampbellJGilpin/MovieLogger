using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class ViewingsService : IViewingsService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public ViewingsService(IAssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ViewingDto> GetViewingByIdAsync(int viewingId)
    {
        var viewing = await _db.UserMovieViewings
            .Include(v => v.User)
            .Include(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(v => v.Review)
            .FirstOrDefaultAsync(v => v.Id == viewingId);

        if (viewing == null)
        {
            throw new KeyNotFoundException($"Viewing with ID {viewingId} not found.");
        }

        return _mapper.Map<ViewingDto>(viewing);
    }

    public async Task<ViewingDto> CreateViewingAsync(int userId, ViewingDto viewingDto)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        // Check if movie exists
        var movieExists = await _db.Movies.AnyAsync(m => m.Id == viewingDto.MovieId && !m.IsDeleted);
        if (!movieExists)
        {
            throw new KeyNotFoundException($"Movie with ID {viewingDto.MovieId} not found.");
        }

        // Create new viewing - no longer requires library membership
        var viewing = new UserMovieViewing
        {
            UserId = userId,
            MovieId = viewingDto.MovieId,
            DateViewed = viewingDto.DateViewed
        };

        _db.UserMovieViewings.Add(viewing);
        await _db.SaveChangesAsync();

        // Return the created viewing with related data
        var createdViewing = await _db.UserMovieViewings
            .Include(v => v.User)
            .Include(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .FirstOrDefaultAsync(v => v.Id == viewing.Id);

        return _mapper.Map<ViewingDto>(createdViewing!);
    }

    public async Task<IEnumerable<ViewingDto>> GetViewingsByUserIdAsync(int userId)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var viewings = await _db.UserMovieViewings
            .Include(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(v => v.Review)
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.DateViewed)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ViewingDto>>(viewings);
    }

    public async Task<(IEnumerable<ViewingDto> Items, int TotalCount, int TotalPages)> GetViewingsByUserIdPaginatedAsync(int userId, int page = 1, int pageSize = 10)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var totalCount = await _db.UserMovieViewings
            .Where(v => v.UserId == userId)
            .CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var viewings = await _db.UserMovieViewings
            .Include(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(v => v.Review)
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.DateViewed)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (_mapper.Map<IEnumerable<ViewingDto>>(viewings), totalCount, totalPages);
    }

    public async Task<IEnumerable<ViewingDto>> GetViewingsForMovieByUserIdAsync(int userId, int movieId)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var movieExists = await _db.Movies.AnyAsync(m => m.Id == movieId && !m.IsDeleted);
        if (!movieExists)
        {
            throw new KeyNotFoundException($"Movie with ID {movieId} not found.");
        }

        var viewings = await _db.UserMovieViewings
            .Include(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(v => v.Review)
            .Where(v => v.UserId == userId && v.MovieId == movieId)
            .OrderByDescending(v => v.DateViewed)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ViewingDto>>(viewings);
    }

    public async Task<ViewingDto> UpdateViewingAsync(int viewingId, ViewingDto viewingDto)
    {
        var viewing = await _db.UserMovieViewings
            .FirstOrDefaultAsync(v => v.Id == viewingId);

        if (viewing == null)
        {
            throw new KeyNotFoundException($"Viewing with ID {viewingId} not found.");
        }

        viewing.DateViewed = viewingDto.DateViewed;

        await _db.SaveChangesAsync();

        // Return updated viewing with related data
        var updatedViewing = await _db.UserMovieViewings
            .Include(v => v.User)
            .Include(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(v => v.Review)
            .FirstOrDefaultAsync(v => v.Id == viewingId);

        return _mapper.Map<ViewingDto>(updatedViewing!);
    }

    public async Task<bool> DeleteViewingAsync(int viewingId)
    {
        var viewing = await _db.UserMovieViewings
            .Include(v => v.Review)
            .FirstOrDefaultAsync(v => v.Id == viewingId);

        if (viewing == null)
        {
            return false;
        }

        // Delete associated review if it exists
        if (viewing.Review != null)
        {
            _db.Reviews.Remove(viewing.Review);
        }

        // Delete the viewing
        _db.UserMovieViewings.Remove(viewing);
        await _db.SaveChangesAsync();

        return true;
    }
}