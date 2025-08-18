namespace movielogger.dal.dtos;

public class ListDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int MovieCount { get; set; }
    public IEnumerable<MovieDto>? Movies { get; set; }
}