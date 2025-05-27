using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class LibraryService : ILibraryService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public LibraryService(IAssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<LibraryDto> GetLibraryByUserIdAsync(int userId)
    {
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
        var entries = await _db.UserMovies
            .Include(um => um.Movie)
            .ThenInclude(m => m.Genre)
            .Where(um => um.UserId == userId && um.UpcomingViewDate != null)
            .ToListAsync();

        var dto = new LibraryDto { UserId = userId, LibraryItems = _mapper.Map<List<LibraryItemDto>>(entries) };
        return dto;
    }

    public async Task<LibraryItemDto> CreateLibraryEntryAsync(int userId, LibraryItemDto libraryItemDto)
    {
        var exists = await _db.UserMovies.AnyAsync(um => um.UserId == userId && um.MovieId == libraryItemDto.MovieId);
        if (exists)
        {
            throw new InvalidOperationException("Library entry already exists for this user and movie.");
        }

        var entry = _mapper.Map<UserMovie>(libraryItemDto);
        entry.UserId = userId;

        _db.UserMovies.Add(entry);
        await _db.SaveChangesAsync();

        return _mapper.Map<LibraryItemDto>(entry);
    }

    public async Task<LibraryItemDto> UpdateLibraryEntryAsync(int userId, LibraryItemDto libraryItemDto)
    {
        var entry = await _db.UserMovies.FirstOrDefaultAsync(um => um.MovieId == libraryItemDto.MovieId && um.UserId == userId);
        if (entry == null)
        {
            throw new KeyNotFoundException("Library entry not found for this user.");
        }

        _mapper.Map(libraryItemDto, entry);
        entry.UserId = userId;

        await _db.SaveChangesAsync();

        return _mapper.Map<LibraryItemDto>(entry);
    }
}
