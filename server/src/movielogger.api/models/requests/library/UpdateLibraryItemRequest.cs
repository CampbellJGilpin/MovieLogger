namespace movielogger.api.models.requests.library;

public class UpdateLibraryItemRequest
{
    public int MovieId { get; set; }
    public bool Favourite { get; set; }
    public bool OwnsMovie  { get; set; }
    public DateTime? UpcomingViewDate { get; set; }
}