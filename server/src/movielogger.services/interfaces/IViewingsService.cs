using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IViewingsService
{
    Task<ViewingDto> GetViewingByIdAsync(int viewingId);
    Task<ViewingDto> CreateViewingAsync(int userId, ViewingDto viewing);
    Task<ViewingDto> UpdateViewingAsync(int viewingId, ViewingDto viewingDto);
    Task<List<ViewingDto>> GetViewingsByUserIdAsync(int userId);
}