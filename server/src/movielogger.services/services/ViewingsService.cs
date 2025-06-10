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
        var viewing = await _db.Viewings
            .Include(v => v.UserMovie)
            .ThenInclude(um => um.Movie)
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
        // Check if UserMovie exists
        var userMovie = await _db.UserMovies
            .FirstOrDefaultAsync(um => um.MovieId == viewingDto.MovieId && um.UserId == userId);

        if (userMovie == null)
        {
            userMovie = new UserMovie
            {
                UserId = userId,
                MovieId = viewingDto.MovieId,
                Favourite = false,
                OwnsMovie = false,
                UpcomingViewDate = null
            };

            _db.UserMovies.Add(userMovie);
            await _db.SaveChangesAsync();
        }

        var viewing = new Viewing { UserMovieId = userMovie.Id, DateViewed = viewingDto.DateViewed };
        
        _db.Viewings.Add(viewing);
        await _db.SaveChangesAsync();

        // Reload viewing with eager includes
        var savedViewing = await _db.Viewings
            .Include(v => v.UserMovie)
            .ThenInclude(um => um.Movie)
            .FirstOrDefaultAsync(v => v.Id == viewing.Id);
        
        return _mapper.Map<ViewingDto>(savedViewing);
    }
    
    public async Task<ViewingDto> UpdateViewingAsync(int viewingId, ViewingDto viewingDto)
    {
        var viewing = await _db.Viewings
            .Include(v => v.UserMovie)
            .ThenInclude(um => um.Movie)
            .FirstOrDefaultAsync(v => v.Id == viewingId);

        if (viewing == null)
        {
            throw new KeyNotFoundException($"Viewing with ID {viewingId} not found.");
        }

        viewing.DateViewed = viewingDto.DateViewed;
        await _db.SaveChangesAsync();

        return _mapper.Map<ViewingDto>(viewing);
    }

    public async Task<List<ViewingDto>> GetViewingsByUserIdAsync(int userId)
    {
        // First check if user exists
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var viewings = await _db.Viewings
            .Include(v => v.UserMovie)
                .ThenInclude(um => um.Movie)
            .Include(v => v.Review)
            .Where(v => v.UserMovie.UserId == userId)
            .ToListAsync();

        return viewings.Select(v => _mapper.Map<ViewingDto>(v)).ToList();
    }
}
