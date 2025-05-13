namespace movielogger.api.models;

public class CreateReviewRequest
{
    public string ReviewText { get; set; } = string.Empty;
    public int Score { get; set; }
}