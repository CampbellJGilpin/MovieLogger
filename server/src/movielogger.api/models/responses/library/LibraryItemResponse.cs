namespace movielogger.api.models.responses.library;

public class LibraryItemResponse
{
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Favourite { get; set; } = string.Empty;
    public string InLibrary { get; set; } = string.Empty;
    public string WatchLater { get; set; } = string.Empty;
}