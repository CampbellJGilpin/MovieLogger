using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface ILibraryService
{
    Task<LibraryDto> GetLibraryByUserIdAsync(int userId);
    Task<LibraryDto> GetLibraryFavouritesByUserIdAsync(int userId);
    Task<LibraryDto> GetLibraryWatchlistByUserIdAsync(int userId);
    Task<LibraryItemDto> CreateLibraryEntryAsync(LibraryItemDto libraryItemDto);
    Task<LibraryItemDto> UpdateLibraryEntryAsync(LibraryItemDto libraryItemDto);
}