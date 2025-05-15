namespace movielogger.api.models.requests.viewings;

public class UpdateViewingRequest
{
    public int MovieId { get; set; }
    public DateTime DateViewed { get; set; }
}