namespace movielogger.dal.entities;

public class Review
{
    public int Id { get; set; }

    public int ViewingId { get; set; }
    public Viewing Viewing { get; set; } = default!;

    public string? ReviewText { get; set; }
    public int? Score { get; set; }  // Optional
}
