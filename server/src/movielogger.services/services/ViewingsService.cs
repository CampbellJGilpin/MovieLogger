using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class ViewingsService : IViewingsService
{
    public Task<ViewingDto> GetViewingAsync(int viewingId)
    {
        throw new NotImplementedException();
    }

    public Task<ViewingDto> CreateViewingAsync(int userId, ViewingDto viewing)
    {
        throw new NotImplementedException();
    }
}