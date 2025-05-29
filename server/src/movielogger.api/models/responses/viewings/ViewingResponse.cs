using movielogger.api.models.responses.movies;

namespace movielogger.api.models.responses.viewings;

public class ViewingResponse
{
    public int ViewingId { get; set; }
    public int UserId { get; set; }
    public bool Favourite { get; set; }
    public bool OwnsMovies { get; set; }
    public DateTime UpcomingViewDate { get; set; }
    public MovieResponse Movie { get; set; }
}
