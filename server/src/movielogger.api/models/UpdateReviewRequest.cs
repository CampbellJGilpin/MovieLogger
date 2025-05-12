namespace movielogger.api.models;

public class UpdateReviewRequest
{
    public string ReviewText { get; set; } = string.Empty;
    public int Score { get; set; }
}