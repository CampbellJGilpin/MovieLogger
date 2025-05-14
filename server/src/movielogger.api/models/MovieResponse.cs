namespace movielogger.api.models;

public class MovieResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public GenreResponse Genre { get; set; }
    public DateTime ReleaseDate { get; set; }
}