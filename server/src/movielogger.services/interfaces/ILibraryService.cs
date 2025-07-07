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
}