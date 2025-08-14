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

    public async Task<(IEnumerable<MovieDto> Movies, int TotalCount)> GetAllMoviesAsync(int page = 1, int pageSize = 10)
    {
        var query = _db.Movies
            .Include(m => m.Genre)
            .Where(m => !m.IsDeleted);

        var totalCount = await query.CountAsync();

        var movies = await query
            .OrderBy(m => m.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (
            Movies: _mapper.Map<List<MovieDto>>(movies),
            TotalCount: totalCount
        );
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

        var savedMovie = await _db.Movies
            .Include(g => g.Genre)
            .FirstOrDefaultAsync(x => x.Id == movie.Id);

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

    public async Task<(IEnumerable<UserMovieDto> Items, int TotalCount, int TotalPages)> GetAllMoviesForUserAsync(int userId, string? search = null, int page = 1, int pageSize = 10)
    {
        var query = _db.Movies
            .Include(m => m.Genre)
            .Where(m => !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.ToLower();
            query = query.Where(m => m.Title.ToLower().Contains(normalized));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var movies = await query
            .OrderBy(m => m.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get user movies for the retrieved movies
        var movieIds = movies.Select(m => m.Id).ToList();
        var userMovies = await _db.UserMovies
            .Where(um => um.UserId == userId && movieIds.Contains(um.MovieId))
            .ToListAsync();

        var userMoviesDict = userMovies.ToDictionary(um => um.MovieId);

        var items = movies.Select(movie =>
        {
            userMoviesDict.TryGetValue(movie.Id, out var userMovie);
            return new UserMovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                GenreId = movie.GenreId,
                GenreTitle = movie.Genre.Title,
                ReleaseDate = movie.ReleaseDate,
                Description = movie.Description,
                IsDeleted = movie.IsDeleted,
                OwnsMovie = userMovie?.OwnsMovie ?? false,
                IsFavourite = userMovie?.Favourite ?? false
            };
        });

        return (Items: items, TotalCount: totalCount, TotalPages: totalPages);
    }
}