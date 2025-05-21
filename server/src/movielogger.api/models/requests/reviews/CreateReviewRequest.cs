namespace movielogger.api.models.requests.reviews;

public class CreateReviewRequest
{
    public string ReviewText { get; set; } = string.Empty;
    public int Score { get; set; }
    public int ViewingId { get; set; }
}