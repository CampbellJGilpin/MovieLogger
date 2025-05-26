using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IViewingsService
{
    Task<ViewingDto> GetViewingAsync(int viewingId);
    Task<ViewingDto> CreateViewingAsync(int userId, ViewingDto viewing);
    Task<ViewingDto> UpdateViewingAsync(int viewingId, ViewingDto viewingDto);
}