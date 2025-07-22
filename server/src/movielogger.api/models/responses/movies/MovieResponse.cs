using movielogger.api.models.responses.genres;

namespace movielogger.api.models.responses.movies;

public class MovieResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public required GenreResponse Genre { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public string? PosterPath { get; set; }
}