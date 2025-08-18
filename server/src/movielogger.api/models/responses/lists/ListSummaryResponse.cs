namespace movielogger.api.models.responses.lists;

public class ListSummaryResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int MovieCount { get; set; }
}