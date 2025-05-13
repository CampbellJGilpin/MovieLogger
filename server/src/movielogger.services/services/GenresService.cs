using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class GenresService : IGenresService
{
    public Task<IEnumerable<GenreDto>> GetGenresAsync()
    {
        throw new NotImplementedException();
    }

    public Task<GenreDto> GetGenreByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<GenreDto> CreateGenreAsync(GenreDto genre)
    {
        throw new NotImplementedException();
    }

    public Task<GenreDto> UpdateGenreAsync(int id, GenreDto genre)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGenreAsync(int id)
    {
        throw new NotImplementedException();
    }
}