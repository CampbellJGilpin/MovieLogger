using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class GenresService : IGenresService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public GenresService(IAssessmentDbContext db, IMapper mapper)
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
        // Check for duplicate title
        var existingGenre = await _db.Genres.FirstOrDefaultAsync(g => g.Title == genreDto.Title);
        if (existingGenre != null)
        {
            throw new InvalidOperationException($"Genre with title '{genreDto.Title}' already exists.");
        }

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

        // Check for duplicate title (excluding current genre)
        var existingGenre = await _db.Genres.FirstOrDefaultAsync(g => g.Title == genreDto.Title && g.Id != genreId);
        if (existingGenre != null)
        {
            throw new InvalidOperationException($"Genre with title '{genreDto.Title}' already exists.");
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

        // Check if genre has associated movies
        var hasMovies = await _db.Movies.AnyAsync(m => m.GenreId == genreId);
        if (hasMovies)
        {
            throw new InvalidOperationException("Cannot delete genre that has associated movies.");
        }

        _db.Genres.Remove(genre);
        await _db.SaveChangesAsync();
    }
}