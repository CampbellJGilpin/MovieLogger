using movielogger.api.models.responses.genres;

namespace movielogger.api.models.responses.movies;

public class UserMovieResponse : MovieResponse
{
    public bool OwnsMovie { get; set; }
    public bool IsFavourite { get; set; }
} 