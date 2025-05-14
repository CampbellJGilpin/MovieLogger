namespace movielogger.api.models;

public class UpdateViewingRequest
{
    public int MovieId { get; set; }
    public DateTime DateViewed { get; set; }
}