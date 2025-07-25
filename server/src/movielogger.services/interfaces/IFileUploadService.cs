using Microsoft.AspNetCore.Http;

namespace movielogger.services.interfaces;

public interface IFileUploadService
{
    Task<string> UploadPosterAsync(IFormFile posterFile);
    Task<bool> DeletePosterAsync(string posterPath);
    Task<string> GetPosterUrlAsync(string posterPath);
} 