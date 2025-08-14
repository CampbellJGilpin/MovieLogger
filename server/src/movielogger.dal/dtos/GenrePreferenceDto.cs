namespace movielogger.dal.dtos;

public class GenrePreferenceDto
{
    public int GenreId { get; set; }
    public string GenreTitle { get; set; } = string.Empty;
    public int WatchCount { get; set; }
    public double AverageRating { get; set; }
    public int TotalRating { get; set; }
    public double PercentageOfTotalWatches { get; set; }
    public DateTime? LastWatched { get; set; }
    public DateTime? FirstWatched { get; set; }
}

public class GenrePreferencesSummaryDto
{
    public int UserId { get; set; }
    public int TotalMoviesWatched { get; set; }
    public int TotalUniqueGenres { get; set; }
    public List<GenrePreferenceDto> GenrePreferences { get; set; } = new();
    public GenrePreferenceDto? MostWatchedGenre { get; set; }
    public GenrePreferenceDto? HighestRatedGenre { get; set; }
    public GenrePreferenceDto? LeastWatchedGenre { get; set; }
}