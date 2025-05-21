using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class ViewingsService : IViewingsService
{
    private readonly AssessmentDbContext _db;
    private readonly IMapper _mapper;

    public ViewingsService(AssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ViewingDto> GetViewingAsync(int viewingId)
    {
        var viewing = await _db.Viewings
            .Include(v => v.UserMovie)
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
        // Validate that UserMovie exists and belongs to the user
        var userMovie = await _db.UserMovies
            .FirstOrDefaultAsync(um => um.Id == viewingDto.UserMovieId && um.UserId == userId);

        if (userMovie == null)
        {
            throw new InvalidOperationException($"UserMovie with ID {viewingDto.UserMovieId} does not belong to User {userId}.");
        }

        var viewing = _mapper.Map<Viewing>(viewingDto);
        _db.Viewings.Add(viewing);
        await _db.SaveChangesAsync();

        return _mapper.Map<ViewingDto>(viewing);
    }


    public async Task<ViewingDto> UpdateViewingAsync(int userId, ViewingDto viewingDto)
    {
        var viewing = await _db.Viewings
            .Include(v => v.UserMovie)
            .FirstOrDefaultAsync(v => v.Id == viewingDto.Id);

        if (viewing == null)
        {
            throw new KeyNotFoundException($"Viewing with ID {viewingDto.Id} not found.");
        }

        // Check ownership of the associated UserMovie
        if (viewing.UserMovie.UserId != userId || viewing.UserMovie.Id != viewingDto.UserMovieId)
        {
            throw new InvalidOperationException($"User {userId} does not have access to UserMovie {viewingDto.UserMovieId}.");
        }

        _mapper.Map(viewingDto, viewing);
        await _db.SaveChangesAsync();

        return _mapper.Map<ViewingDto>(viewing);
    }

}
