using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IMoviesService
{
    Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
    Task<MovieDto> GetMovieByIdAsync(int id);
    Task<MovieDto> CreateMovieAsync(MovieDto movieDto);
    Task<MovieDto> UpdateMovieAsync(int id, MovieDto movieDto);
    Task<bool> DeleteMovieAsync(int id);
}