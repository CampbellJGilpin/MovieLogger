using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IGenrePreferencesService
{
    Task<GenrePreferencesSummaryDto> GetUserGenrePreferencesAsync(int userId);
    Task<List<GenrePreferenceDto>> GetTopGenresByWatchCountAsync(int userId, int count = 5);
    Task<List<GenrePreferenceDto>> GetTopGenresByRatingAsync(int userId, int count = 5);
    Task<List<GenrePreferenceDto>> GetLeastWatchedGenresAsync(int userId, int count = 5);
    Task<Dictionary<string, int>> GetGenreWatchTrendsAsync(int userId, int months = 6);
} 