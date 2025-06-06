using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IGenresService
{
    Task<IEnumerable<GenreDto>> GetGenresAsync();
    Task<GenreDto> GetGenreByIdAsync(int id);
    Task<GenreDto> CreateGenreAsync(GenreDto genre);
    Task<GenreDto> UpdateGenreAsync(int genreId, GenreDto genre);
    Task DeleteGenreAsync(int genreId);
}