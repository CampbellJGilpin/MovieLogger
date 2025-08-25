using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IViewingsService
{
    Task<ViewingDto> GetViewingByIdAsync(int viewingId);
    Task<ViewingDto> CreateViewingAsync(int userId, ViewingDto viewing);
    Task<ViewingDto> UpdateViewingAsync(int viewingId, ViewingDto viewingDto);
    Task<IEnumerable<ViewingDto>> GetViewingsByUserIdAsync(int userId);
    Task<bool> DeleteViewingAsync(int viewingId);
    Task<IEnumerable<ViewingDto>> GetViewingsForMovieByUserIdAsync(int userId, int movieId);
    Task<(IEnumerable<ViewingDto> Items, int TotalCount, int TotalPages)> GetViewingsByUserIdPaginatedAsync(int userId, int page = 1, int pageSize = 10);
}