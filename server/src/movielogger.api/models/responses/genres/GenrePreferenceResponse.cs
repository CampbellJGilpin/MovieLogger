namespace movielogger.api.models.responses.genres;

public class GenrePreferenceResponse
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

public class GenrePreferencesSummaryResponse
{
    public int UserId { get; set; }
    public int TotalMoviesWatched { get; set; }
    public int TotalUniqueGenres { get; set; }
    public List<GenrePreferenceResponse> GenrePreferences { get; set; } = new();
    public GenrePreferenceResponse? MostWatchedGenre { get; set; }
    public GenrePreferenceResponse? HighestRatedGenre { get; set; }
    public GenrePreferenceResponse? LeastWatchedGenre { get; set; }
}