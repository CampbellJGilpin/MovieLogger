namespace movielogger.api.models.responses.library;

public class LibraryResponse
{
    public int UserId { get; set; }
    public ICollection<LibraryItemResponse> LibraryItems { get; set; } = new List<LibraryItemResponse>();
}