namespace movielogger.api.models;

public class CreateViewingRequest
{
    public int MovieId { get; set; }
    public DateTime DateViewed { get; set; }
}