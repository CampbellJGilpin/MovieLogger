namespace movielogger.api.models;

public class UpdateMovieRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; } = DateTime.Today;
    public int GenreId { get; set; }
}