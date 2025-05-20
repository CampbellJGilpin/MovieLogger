namespace movielogger.api.models.responses.reviews;

public class ReviewResponse
{
    public int Id { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime DateViewed { get; set; }
    public int Score { get; set; }
    public string ReviewText { get; set; } = string.Empty;
}