using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IMoviesService
{
    Task<(IEnumerable<MovieDto> Movies, int TotalCount)> GetAllMoviesAsync(int page = 1, int pageSize = 10);
    Task<MovieDto> GetMovieByIdAsync(int movieId);
    Task<MovieDto> CreateMovieAsync(MovieDto dto);
    Task<MovieDto> UpdateMovieAsync(int movieId, MovieDto dto);
    Task<bool> DeleteMovieAsync(int movieId);
    Task<IEnumerable<MovieDto>> SearchMoviesAsync(string query);
}