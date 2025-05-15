namespace movielogger.api.models.requests.genres;

public class UpdateGenreRequest
{
    public int GenreId { get; set; }
    public string Title { get; set; } = string.Empty;
}