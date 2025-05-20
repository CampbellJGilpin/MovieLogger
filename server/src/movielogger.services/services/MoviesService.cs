using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class MoviesService : IMoviesService
{
    private readonly AssessmentDbContext _db;
    private readonly IMapper _mapper;

    public MoviesService(AssessmentDbContext db, IMapper mapper)
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
        await _db.SaveChangesAsync();

        return _mapper.Map<MovieDto>(movie);
    }

    public async Task<MovieDto> UpdateMovieAsync(int movieId, MovieDto dto)
    {
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieId && !m.IsDeleted);
        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {movieId} not found.");
        }

        _mapper.Map(dto, movie);
        await _db.SaveChangesAsync();

        return _mapper.Map<MovieDto>(movie);
    }

    public async Task<bool> DeleteMovieAsync(int movieId)
    {
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieId && !m.IsDeleted);
        if (movie == null) return false;

        movie.IsDeleted = true;
        await _db.SaveChangesAsync();

        return true;
    }
}