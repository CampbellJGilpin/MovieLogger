using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class GenrePreferencesService : IGenrePreferencesService
{
    private readonly IAssessmentDbContext _db;

    public GenrePreferencesService(IAssessmentDbContext db)
    {
        _db = db;
    }

    public async Task<GenrePreferencesSummaryDto> GetUserGenrePreferencesAsync(int userId)
    {
        // Check if user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        // Get all viewings for the user with movie and genre data
        var viewings = await _db.UserMovieViewings
            .Include(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Include(v => v.Review)
            .Where(v => v.UserId == userId)
            .ToListAsync();

        if (!viewings.Any())
        {
            return new GenrePreferencesSummaryDto
            {
                UserId = userId,
                TotalMoviesWatched = 0,
                TotalUniqueGenres = 0,
                GenrePreferences = new List<GenrePreferenceDto>()
            };
        }

        // Group by genre and calculate statistics
        var genreStats = viewings
            .GroupBy(v => v.Movie.Genre)
            .Select(g => new GenrePreferenceDto
            {
                GenreId = g.Key.Id,
                GenreTitle = g.Key.Title,
                WatchCount = g.Count(),
                AverageRating = g.Where(v => v.Review != null && v.Review.Score > 0)
                    .Average(v => v.Review!.Score) ?? 0.0,
                TotalRating = g.Where(v => v.Review != null && v.Review.Score > 0)
                    .Sum(v => v.Review!.Score) ?? 0,
                LastWatched = g.Max(v => v.DateViewed),
                FirstWatched = g.Min(v => v.DateViewed)
            })
            .ToList();

        var totalWatches = viewings.Count();

        // Calculate percentages
        foreach (var genre in genreStats)
        {
            genre.PercentageOfTotalWatches = totalWatches > 0
                ? Math.Round((double)genre.WatchCount / totalWatches * 100, 1)
                : 0;
        }

        // Sort by watch count for most/least watched
        var sortedByWatchCount = genreStats.OrderByDescending(g => g.WatchCount).ToList();
        var sortedByRating = genreStats.Where(g => g.AverageRating > 0)
            .OrderByDescending(g => g.AverageRating).ToList();

        return new GenrePreferencesSummaryDto
        {
            UserId = userId,
            TotalMoviesWatched = totalWatches,
            TotalUniqueGenres = genreStats.Count,
            GenrePreferences = genreStats.OrderByDescending(g => g.WatchCount).ToList(),
            MostWatchedGenre = sortedByWatchCount.FirstOrDefault(),
            HighestRatedGenre = sortedByRating.FirstOrDefault(),
            LeastWatchedGenre = sortedByWatchCount.LastOrDefault()
        };
    }

    public async Task<List<GenrePreferenceDto>> GetTopGenresByWatchCountAsync(int userId, int count = 5)
    {
        var summary = await GetUserGenrePreferencesAsync(userId);
        return summary.GenrePreferences.Take(count).ToList();
    }

    public async Task<List<GenrePreferenceDto>> GetTopGenresByRatingAsync(int userId, int count = 5)
    {
        var summary = await GetUserGenrePreferencesAsync(userId);
        return summary.GenrePreferences
            .Where(g => g.AverageRating > 0)
            .OrderByDescending(g => g.AverageRating)
            .Take(count)
            .ToList();
    }

    public async Task<List<GenrePreferenceDto>> GetLeastWatchedGenresAsync(int userId, int count = 5)
    {
        var summary = await GetUserGenrePreferencesAsync(userId);
        return summary.GenrePreferences
            .OrderBy(g => g.WatchCount)
            .Take(count)
            .ToList();
    }

    public async Task<Dictionary<string, int>> GetGenreWatchTrendsAsync(int userId, int months = 6)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-months);

        var monthlyTrends = await _db.UserMovieViewings
            .Include(v => v.Movie)
            .ThenInclude(m => m.Genre)
            .Where(v => v.UserId == userId && v.DateViewed >= cutoffDate)
            .GroupBy(v => new { Month = v.DateViewed.Month, Year = v.DateViewed.Year, Genre = v.Movie.Genre.Title })
            .Select(g => new
            {
                MonthYear = $"{g.Key.Year}-{g.Key.Month:D2}",
                Genre = g.Key.Genre,
                Count = g.Count()
            })
            .ToListAsync();

        var result = new Dictionary<string, int>();
        foreach (var trend in monthlyTrends)
        {
            var key = $"{trend.MonthYear}_{trend.Genre}";
            result[key] = trend.Count;
        }

        return result;
    }
}