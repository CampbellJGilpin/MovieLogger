namespace movielogger.dal.dtos;

public class ReviewDto
{
    public int Id { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime DateViewed { get; set; }
    public int Score { get; set; }
    public string ReviewText { get; set; } = string.Empty;
    public int ViewingId { get; set; }
}