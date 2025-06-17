namespace movielogger.dal.dtos;

public class LibraryItemDto
{
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; } = string.Empty;
    public bool Favourite { get; set; }
    public bool InLibrary { get; set; }
    public bool WatchLater { get; set; }
    public bool OwnsMovie { get; set; }
}