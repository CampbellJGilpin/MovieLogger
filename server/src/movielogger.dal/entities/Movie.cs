namespace movielogger.dal.entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public bool IsDeleted { get; set; } = false;

    public int GenreId { get; set; }
    public Genre Genre { get; set; } = default!;

    public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
}