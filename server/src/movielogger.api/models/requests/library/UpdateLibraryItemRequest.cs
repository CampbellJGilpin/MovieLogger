namespace movielogger.api.models.requests.library;

public class UpdateLibraryItemRequest
{
    public int LibraryId { get; set; }
    public int MovieId { get; set; }
    public bool Favorite { get; set; }
    public bool OwnsMovie  { get; set; }
    public DateTime? UpcomingViewDate { get; set; }
}