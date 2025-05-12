namespace movielogger.api.models;

public class AddReviewRequest
{
    public string ReviewText { get; set; } = string.Empty;
    public int Score { get; set; }
}