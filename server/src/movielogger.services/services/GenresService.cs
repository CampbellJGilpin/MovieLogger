using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class GenresService : IGenresService
{
    private readonly AssessmentDbContext _db;
    private readonly IMapper _mapper;

    public GenresService(AssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GenreDto>> GetGenresAsync()
    {
        var genres = await _db.Genres.ToListAsync();
        return _mapper.Map<IEnumerable<GenreDto>>(genres);
    }

    public async Task<GenreDto> GetGenreByIdAsync(int id)
    {
        var genre = await _db.Genres.FindAsync(id);
        if (genre == null)
        {
            throw new KeyNotFoundException($"Genre with ID {id} not found.");
        }

        return _mapper.Map<GenreDto>(genre);
    }

    public async Task<GenreDto> CreateGenreAsync(GenreDto genreDto)
    {
        var genre = _mapper.Map<Genre>(genreDto);
        
        genre.Id = 0;
        
        _db.Genres.Add(genre);
        await _db.SaveChangesAsync();

        return _mapper.Map<GenreDto>(genre);
    }

    public async Task<GenreDto> UpdateGenreAsync(int genreId, GenreDto genreDto)
    {
        var genre = await _db.Genres.FindAsync(genreId);
        if (genre == null)
        {
            throw new KeyNotFoundException($"Genre with ID {genreId} not found.");
        }

        genre.Id = genreId;
        
        _mapper.Map(genreDto, genre);
        await _db.SaveChangesAsync();

        return _mapper.Map<GenreDto>(genre);
    }

    public async Task DeleteGenreAsync(int genreId)
    {
        var genre = await _db.Genres.FindAsync(genreId);
        if (genre == null)
        {
            throw new KeyNotFoundException($"Genre with ID {genreId} not found.");
        }

        _db.Genres.Remove(genre);
        await _db.SaveChangesAsync();
    }
}