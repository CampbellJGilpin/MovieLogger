using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IMovieQueryService
{
    Task<(IEnumerable<MovieDto> Movies, int TotalCount)> GetAllMoviesAsync(int page = 1, int pageSize = 10);
    Task<MovieDto> GetMovieByIdAsync(int movieId);
    Task<IEnumerable<MovieDto>> SearchMoviesAsync(string query);
    Task<(IEnumerable<UserMovieDto> Items, int TotalCount, int TotalPages)> GetAllMoviesForUserAsync(int userId, string? search = null, int page = 1, int pageSize = 10);
}