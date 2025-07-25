using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class FileUploadService : IFileUploadService
{
    private readonly IHostingEnvironment _env;
    private readonly IConfiguration _configuration;

    public FileUploadService(IHostingEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    public async Task<string> UploadPosterAsync(IFormFile posterFile)
    {
        if (posterFile == null || posterFile.Length == 0)
            throw new ArgumentException("Poster file is required");

        if (_env.IsDevelopment())
        {
            return await SaveToLocalAsync(posterFile);
        }
        else
        {
            return await UploadToS3Async(posterFile);
        }
    }

    public async Task<bool> DeletePosterAsync(string posterPath)
    {
        if (string.IsNullOrEmpty(posterPath))
            return false;

        if (_env.IsDevelopment())
        {
            return await DeleteFromLocalAsync(posterPath);
        }
        else
        {
            return await DeleteFromS3Async(posterPath);
        }
    }

    public Task<string> GetPosterUrlAsync(string posterPath)
    {
        if (string.IsNullOrEmpty(posterPath))
            return Task.FromResult(string.Empty);

        if (_env.IsDevelopment())
        {
            return Task.FromResult(posterPath);
        }
        else
        {
            return Task.FromResult($"{_configuration["S3:BaseUrl"]}/{posterPath}");
        }
    }

    private async Task<string> SaveToLocalAsync(IFormFile file)
    {
        var uploadsDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{fileName}";
    }

    private Task<string> UploadToS3Async(IFormFile file)
    {
        // TODO: Implement S3 upload logic
        // This would use AWS SDK to upload to S3
        throw new NotImplementedException("S3 upload not yet implemented");
    }

    private Task<bool> DeleteFromLocalAsync(string posterPath)
    {
        try
        {
            var filePath = Path.Combine(_env.ContentRootPath, "wwwroot", posterPath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private Task<bool> DeleteFromS3Async(string posterPath)
    {
        // TODO: Implement S3 delete logic
        throw new NotImplementedException("S3 delete not yet implemented");
    }
} 