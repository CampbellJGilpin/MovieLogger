using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class MoviesService : IMoviesService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public MoviesService(IAssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
    {
        var movies = await _db.Movies
            .Include(m => m.Genre)
            .Where(m => !m.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<MovieDto>>(movies);
    }

    public async Task<MovieDto> GetMovieByIdAsync(int movieId)
    {
        var movie = await _db.Movies
            .Include(m => m.Genre)
            .FirstOrDefaultAsync(m => m.Id == movieId && !m.IsDeleted);

        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {movieId} not found.");
        }

        return _mapper.Map<MovieDto>(movie);
    }

    public async Task<MovieDto> CreateMovieAsync(MovieDto dto)
    {
        var movie = _mapper.Map<Movie>(dto);
        _db.Movies.Add(movie);
        var movieId = await _db.SaveChangesAsync();

        var savedMovie = await _db.Movies
            .Include(g => g.Genre)
            .FirstOrDefaultAsync(x => x.Id == movieId);
        
        return _mapper.Map<MovieDto>(savedMovie);
    }

    public async Task<MovieDto> UpdateMovieAsync(int movieId, MovieDto dto)
    {
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieId && !m.IsDeleted);
        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {movieId} not found.");
        }

        dto.Id = movieId;
        
        _mapper.Map(dto, movie);
        await _db.SaveChangesAsync();

        return _mapper.Map<MovieDto>(movie);
    }

    public async Task<bool> DeleteMovieAsync(int movieId)
    {
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieId && !m.IsDeleted);
        if (movie == null)
        {
            return false;
        }

        movie.IsDeleted = true;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string query)
    {
        var normalizedQuery = query.ToLower();
        var movies = await _db.Movies
            .Include(m => m.Genre)
            .Where(m => !m.IsDeleted && m.Title.ToLower().Contains(normalizedQuery))
            .ToListAsync();

        return _mapper.Map<List<MovieDto>>(movies);
    }
}