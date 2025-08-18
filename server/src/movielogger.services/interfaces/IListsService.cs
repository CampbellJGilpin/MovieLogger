using movielogger.dal.dtos;
using movielogger.services.models;

namespace movielogger.services.interfaces;

public interface IListsService
{
    Task<Result<IEnumerable<ListDto>>> GetUserListsAsync(int userId);
    Task<Result<ListDto?>> GetListByIdAsync(int listId, int userId);
    Task<Result<ListDto>> CreateListAsync(int userId, string name, string? description = null);
    Task<Result<ListDto>> UpdateListAsync(int listId, int userId, string name, string? description = null);
    Task<Result<bool>> DeleteListAsync(int listId, int userId);
    Task<Result<bool>> AddMovieToListAsync(int listId, int movieId, int userId);
    Task<Result<bool>> RemoveMovieFromListAsync(int listId, int movieId, int userId);
    Task<Result<IEnumerable<MovieDto>>> GetListMoviesAsync(int listId, int userId);
    Task<Result<bool>> IsMovieInListAsync(int listId, int movieId, int userId);
}