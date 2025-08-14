using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class LibraryService : ILibraryService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(IAssessmentDbContext db, IMapper mapper, ILogger<LibraryService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LibraryDto> GetLibraryByUserIdAsync(int userId)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var entries = await _db.UserMovies
            .Include(um => um.Movie)
            .ThenInclude(m => m.Genre)
            .Where(um => um.UserId == userId)
            .ToListAsync();

        var dto = new LibraryDto { UserId = userId, LibraryItems = _mapper.Map<List<LibraryItemDto>>(entries) };
        return dto;
    }

    public async Task<LibraryDto> GetLibraryFavouritesByUserIdAsync(int userId)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var entries = await _db.UserMovies
            .Include(um => um.Movie)
            .ThenInclude(m => m.Genre)
            .Where(um => um.UserId == userId && um.Favourite)
            .ToListAsync();

        var dto = new LibraryDto { UserId = userId, LibraryItems = _mapper.Map<List<LibraryItemDto>>(entries) };
        return dto;
    }

    public async Task<LibraryDto> GetLibraryWatchlistByUserIdAsync(int userId)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var entries = await _db.UserMovies
            .Include(um => um.Movie)
            .ThenInclude(m => m.Genre)
            .Where(um => um.UserId == userId && um.UpcomingViewDate != null)
            .ToListAsync();

        var dto = new LibraryDto { UserId = userId, LibraryItems = _mapper.Map<List<LibraryItemDto>>(entries) };
        return dto;
    }

    public async Task<LibraryItemDto> UpdateLibraryEntryAsync(int userId, LibraryItemDto libraryItemDto)
    {
        try
        {
            _logger.LogInformation("Updating library entry for user {UserId} and movie {MovieId}", userId, libraryItemDto.MovieId);

            // Check if the user exists
            var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
            if (!userExists)
            {
                _logger.LogWarning("User {UserId} not found or is deleted", userId);
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            // Check if the movie exists
            var movieExists = await _db.Movies.AnyAsync(m => m.Id == libraryItemDto.MovieId && !m.IsDeleted);
            if (!movieExists)
            {
                _logger.LogWarning("Movie {MovieId} not found or is deleted", libraryItemDto.MovieId);
                throw new KeyNotFoundException($"Movie with ID {libraryItemDto.MovieId} not found.");
            }

            // Try to find existing entry
            var entry = await _db.UserMovies
                .Include(um => um.Movie)
                .ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync(um => um.MovieId == libraryItemDto.MovieId && um.UserId == userId);

            if (entry == null)
            {
                // Create new entry
                _logger.LogInformation("Creating new library entry for user {UserId} and movie {MovieId}", userId, libraryItemDto.MovieId);
                entry = _mapper.Map<UserMovie>(libraryItemDto);
                entry.UserId = userId;
                _db.UserMovies.Add(entry);
            }
            else
            {
                // Update existing entry
                _logger.LogInformation("Current entry state: Favourite={Favourite}, OwnsMovie={OwnsMovie}", entry.Favourite, entry.OwnsMovie);
                _mapper.Map(libraryItemDto, entry);
                entry.UserId = userId;
            }

            _logger.LogInformation("New entry state: Favourite={Favourite}, OwnsMovie={OwnsMovie}", entry.Favourite, entry.OwnsMovie);
            await _db.SaveChangesAsync();

            // Reload the entry to get updated navigation properties
            entry = await _db.UserMovies
                .Include(um => um.Movie)
                .ThenInclude(m => m.Genre)
                .FirstAsync(um => um.Id == entry.Id);

            var result = _mapper.Map<LibraryItemDto>(entry);
            _logger.LogInformation("Updated library entry for movie {MovieId}", libraryItemDto.MovieId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating library entry for user {UserId} and movie {MovieId}", userId, libraryItemDto.MovieId);
            throw;
        }
    }

    public async Task<LibraryItemDto?> GetLibraryItemAsync(int userId, int movieId)
    {
        try
        {
            _logger.LogInformation("Getting library item for user {UserId} and movie {MovieId}", userId, movieId);

            var entry = await _db.UserMovies
                .Include(um => um.Movie)
                .ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync(um => um.MovieId == movieId && um.UserId == userId);

            if (entry != null)
            {
                _logger.LogInformation("Found library item. Favourite={Favourite}, OwnsMovie={OwnsMovie}", entry.Favourite, entry.OwnsMovie);
            }
            else
            {
                _logger.LogInformation("No library item found");
            }

            return entry == null ? null : _mapper.Map<LibraryItemDto>(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting library item for user {UserId} and movie {MovieId}", userId, movieId);
            throw;
        }
    }

    public async Task<bool> RemoveFromLibraryAsync(int userId, int movieId)
    {
        try
        {
            _logger.LogInformation("Removing movie {MovieId} from library for user {UserId}", movieId, userId);

            var entry = await _db.UserMovies
                .FirstOrDefaultAsync(um => um.MovieId == movieId && um.UserId == userId);

            if (entry == null)
            {
                _logger.LogWarning("Library item not found for user {UserId} and movie {MovieId}", userId, movieId);
                return false;
            }

            _db.UserMovies.Remove(entry);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully removed movie {MovieId} from library for user {UserId}", movieId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing movie {MovieId} from library for user {UserId}", movieId, userId);
            throw;
        }
    }

    public async Task<(IEnumerable<LibraryItemDto> Items, int TotalCount, int TotalPages)> GetLibraryByUserIdPaginatedAsync(int userId, int page = 1, int pageSize = 10)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var query = _db.UserMovies
            .Include(um => um.Movie)
            .ThenInclude(m => m.Genre)
            .Where(um => um.UserId == userId && um.OwnsMovie);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var entries = await query
            .OrderBy(um => um.Movie.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = _mapper.Map<List<LibraryItemDto>>(entries);
        return (Items: items, TotalCount: totalCount, TotalPages: totalPages);
    }

    public async Task<(IEnumerable<LibraryItemDto> Items, int TotalCount, int TotalPages)> GetLibraryFavouritesByUserIdPaginatedAsync(int userId, int page = 1, int pageSize = 10)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var query = _db.UserMovies
            .Include(um => um.Movie)
            .ThenInclude(m => m.Genre)
            .Where(um => um.UserId == userId && um.Favourite);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var entries = await query
            .OrderBy(um => um.Movie.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = _mapper.Map<List<LibraryItemDto>>(entries);
        return (Items: items, TotalCount: totalCount, TotalPages: totalPages);
    }
}
