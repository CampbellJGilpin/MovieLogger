namespace movielogger.api.models.requests.viewings;

public class CreateViewingRequest
{
    public int MovieId { get; set; }
    public DateTime DateViewed { get; set; }
}