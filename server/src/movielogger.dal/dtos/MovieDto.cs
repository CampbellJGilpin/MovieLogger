namespace movielogger.dal.dtos;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public required GenreDto  Genre { get; set; }
    public int GenreId { get; set; }
    public bool IsDeleted { get; set; }
    public string? PosterPath { get; set; }
}