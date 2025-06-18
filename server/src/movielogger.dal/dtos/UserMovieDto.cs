namespace movielogger.dal.dtos;

public class UserMovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public string GenreTitle { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public bool OwnsMovie { get; set; }
    public bool IsFavourite { get; set; }
} 