using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface ILibraryService
{
    Task<LibraryDto> GetLibraryByUserIdAsync(int userId);
    Task<LibraryDto> GetLibraryFavouritesByUserIdAsync(int userId);
    Task<LibraryDto> GetLibraryWatchlistByUserIdAsync(int userId);
    Task<LibraryItemDto> UpdateLibraryEntryAsync(int userId, LibraryItemDto libraryItemDto);
    Task<LibraryItemDto?> GetLibraryItemAsync(int userId, int movieId);
    Task<bool> RemoveFromLibraryAsync(int userId, int movieId);
    
    // New paginated methods
    Task<(IEnumerable<LibraryItemDto> Items, int TotalCount, int TotalPages)> GetLibraryByUserIdPaginatedAsync(int userId, int page = 1, int pageSize = 10);
    Task<(IEnumerable<LibraryItemDto> Items, int TotalCount, int TotalPages)> GetLibraryFavouritesByUserIdPaginatedAsync(int userId, int page = 1, int pageSize = 10);
}