using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IMoviesService
{
    Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
    Task<MovieDto> GetMovieByIdAsync(int movieId);
    Task<MovieDto> CreateMovieAsync(MovieDto dto);
    Task<MovieDto> UpdateMovieAsync(int movieId, MovieDto dto);
    Task<bool> DeleteMovieAsync(int movieId);
}