using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.interfaces;
using movielogger.services.models;

namespace movielogger.services.services;

public class ListsService : IListsService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public ListsService(IAssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ListDto>>> GetUserListsAsync(int userId)
    {
        try
        {
            var lists = await _db.Lists
                .Where(l => l.UserId == userId && !l.IsDeleted)
                .Include(l => l.ListMovies)
                    .ThenInclude(lm => lm.Movie)
                .OrderByDescending(l => l.UpdatedDate)
                .ToListAsync();

            var listDtos = lists.Select(l => new ListDto
            {
                Id = l.Id,
                UserId = l.UserId,
                Name = l.Name,
                Description = l.Description,
                CreatedDate = l.CreatedDate,
                UpdatedDate = l.UpdatedDate,
                MovieCount = l.ListMovies.Count(lm => !lm.Movie.IsDeleted)
            });

            return Result<IEnumerable<ListDto>>.Success(listDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ListDto>>.Failure($"Failed to retrieve user lists: {ex.Message}");
        }
    }

    public async Task<Result<ListDto?>> GetListByIdAsync(int listId, int userId)
    {
        try
        {
            var list = await _db.Lists
                .Where(l => l.Id == listId && l.UserId == userId && !l.IsDeleted)
                .Include(l => l.ListMovies)
                    .ThenInclude(lm => lm.Movie)
                        .ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync();

            if (list == null)
            {
                return Result<ListDto?>.Success(null);
            }

            var listDto = new ListDto
            {
                Id = list.Id,
                UserId = list.UserId,
                Name = list.Name,
                Description = list.Description,
                CreatedDate = list.CreatedDate,
                UpdatedDate = list.UpdatedDate,
                MovieCount = list.ListMovies.Count(lm => !lm.Movie.IsDeleted),
                Movies = _mapper.Map<IEnumerable<MovieDto>>(list.ListMovies
                    .Where(lm => !lm.Movie.IsDeleted)
                    .OrderByDescending(lm => lm.AddedDate)
                    .Select(lm => lm.Movie))
            };

            return Result<ListDto?>.Success(listDto);
        }
        catch (Exception ex)
        {
            return Result<ListDto?>.Failure($"Failed to retrieve list: {ex.Message}");
        }
    }

    public async Task<Result<ListDto>> CreateListAsync(int userId, string name, string? description = null)
    {
        try
        {
            // Check if user already has a list with this name
            var existingList = await _db.Lists
                .AnyAsync(l => l.UserId == userId && l.Name == name && !l.IsDeleted);

            if (existingList)
            {
                return Result<ListDto>.Failure("A list with this name already exists");
            }

            var list = new List
            {
                UserId = userId,
                Name = name,
                Description = description,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _db.Lists.Add(list);
            await _db.SaveChangesAsync();

            var listDto = new ListDto
            {
                Id = list.Id,
                UserId = list.UserId,
                Name = list.Name,
                Description = list.Description,
                CreatedDate = list.CreatedDate,
                UpdatedDate = list.UpdatedDate,
                MovieCount = 0
            };

            return Result<ListDto>.Success(listDto);
        }
        catch (Exception ex)
        {
            return Result<ListDto>.Failure($"Failed to create list: {ex.Message}");
        }
    }

    public async Task<Result<ListDto>> UpdateListAsync(int listId, int userId, string name, string? description = null)
    {
        try
        {
            var list = await _db.Lists
                .FirstOrDefaultAsync(l => l.Id == listId && l.UserId == userId && !l.IsDeleted);

            if (list == null)
            {
                return Result<ListDto>.Failure("List not found");
            }

            // Check if user has another list with this name
            var existingList = await _db.Lists
                .AnyAsync(l => l.UserId == userId && l.Name == name && l.Id != listId && !l.IsDeleted);

            if (existingList)
            {
                return Result<ListDto>.Failure("A list with this name already exists");
            }

            list.Name = name;
            list.Description = description;
            list.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var listDto = new ListDto
            {
                Id = list.Id,
                UserId = list.UserId,
                Name = list.Name,
                Description = list.Description,
                CreatedDate = list.CreatedDate,
                UpdatedDate = list.UpdatedDate,
                MovieCount = await _db.ListMovies
                    .CountAsync(lm => lm.ListId == listId && !lm.Movie.IsDeleted)
            };

            return Result<ListDto>.Success(listDto);
        }
        catch (Exception ex)
        {
            return Result<ListDto>.Failure($"Failed to update list: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteListAsync(int listId, int userId)
    {
        try
        {
            var list = await _db.Lists
                .FirstOrDefaultAsync(l => l.Id == listId && l.UserId == userId && !l.IsDeleted);

            if (list == null)
            {
                return Result<bool>.Failure("List not found");
            }

            list.IsDeleted = true;
            list.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete list: {ex.Message}");
        }
    }

    public async Task<Result<bool>> AddMovieToListAsync(int listId, int movieId, int userId)
    {
        try
        {
            // Verify list belongs to user
            var list = await _db.Lists
                .FirstOrDefaultAsync(l => l.Id == listId && l.UserId == userId && !l.IsDeleted);

            if (list == null)
            {
                return Result<bool>.Failure("List not found");
            }

            // Verify movie exists
            var movie = await _db.Movies
                .FirstOrDefaultAsync(m => m.Id == movieId && !m.IsDeleted);

            if (movie == null)
            {
                return Result<bool>.Failure("Movie not found");
            }

            // Check if movie is already in list
            var existingListMovie = await _db.ListMovies
                .AnyAsync(lm => lm.ListId == listId && lm.MovieId == movieId);

            if (existingListMovie)
            {
                return Result<bool>.Failure("Movie is already in the list");
            }

            var listMovie = new ListMovie
            {
                ListId = listId,
                MovieId = movieId,
                AddedDate = DateTime.UtcNow
            };

            _db.ListMovies.Add(listMovie);

            // Update list's updated_date
            list.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to add movie to list: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RemoveMovieFromListAsync(int listId, int movieId, int userId)
    {
        try
        {
            // Verify list belongs to user
            var list = await _db.Lists
                .FirstOrDefaultAsync(l => l.Id == listId && l.UserId == userId && !l.IsDeleted);

            if (list == null)
            {
                return Result<bool>.Failure("List not found");
            }

            var listMovie = await _db.ListMovies
                .FirstOrDefaultAsync(lm => lm.ListId == listId && lm.MovieId == movieId);

            if (listMovie == null)
            {
                return Result<bool>.Failure("Movie not found in list");
            }

            _db.ListMovies.Remove(listMovie);

            // Update list's updated_date
            list.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to remove movie from list: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MovieDto>>> GetListMoviesAsync(int listId, int userId)
    {
        try
        {
            // Verify list belongs to user
            var listExists = await _db.Lists
                .AnyAsync(l => l.Id == listId && l.UserId == userId && !l.IsDeleted);

            if (!listExists)
            {
                return Result<IEnumerable<MovieDto>>.Failure("List not found");
            }

            var movies = await _db.ListMovies
                .Where(lm => lm.ListId == listId && !lm.Movie.IsDeleted)
                .Include(lm => lm.Movie)
                    .ThenInclude(m => m.Genre)
                .OrderByDescending(lm => lm.AddedDate)
                .Select(lm => lm.Movie)
                .ToListAsync();

            var movieDtos = _mapper.Map<IEnumerable<MovieDto>>(movies);

            return Result<IEnumerable<MovieDto>>.Success(movieDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MovieDto>>.Failure($"Failed to retrieve list movies: {ex.Message}");
        }
    }

    public async Task<Result<bool>> IsMovieInListAsync(int listId, int movieId, int userId)
    {
        try
        {
            // Verify list belongs to user
            var listExists = await _db.Lists
                .AnyAsync(l => l.Id == listId && l.UserId == userId && !l.IsDeleted);

            if (!listExists)
            {
                return Result<bool>.Failure("List not found");
            }

            var movieInList = await _db.ListMovies
                .AnyAsync(lm => lm.ListId == listId && lm.MovieId == movieId);

            return Result<bool>.Success(movieInList);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to check if movie is in list: {ex.Message}");
        }
    }
}