using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IMovieCommandService
{
    Task<MovieDto> CreateMovieAsync(MovieDto dto);
    Task<MovieDto> UpdateMovieAsync(int movieId, MovieDto dto);
    Task<bool> DeleteMovieAsync(int movieId);
} 