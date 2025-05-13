using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class MoviesService : IMoviesService
{
    public Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MovieDto> GetMovieByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<MovieDto> CreateMovieAsync(MovieDto movieDto)
    {
        throw new NotImplementedException();
    }

    public Task<MovieDto> UpdateMovieAsync(int id, MovieDto movieDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteMovieAsync(int id)
    {
        throw new NotImplementedException();
    }
}