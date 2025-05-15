using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class LibraryService : ILibraryService
{
    public Task<LibraryDto> GetLibraryByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<LibraryDto> GetLibraryFavouritesByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<LibraryDto> GetLibraryWatchlistByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<LibraryItemDto> CreateLibraryEntryAsync(int userId, LibraryItemDto libraryItemDto)
    {
        throw new NotImplementedException();
    }

    public Task<LibraryItemDto> UpdateLibraryEntryAsync(int userId, LibraryItemDto libraryItemDto)
    {
        throw new NotImplementedException();
    }
}