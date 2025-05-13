using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface ILibraryService
{
    Task<LibraryDto> GetLibraryByUserIdAsync(int userId);
}