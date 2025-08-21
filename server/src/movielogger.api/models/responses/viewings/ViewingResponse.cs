using movielogger.api.models.responses.movies;
using movielogger.api.models.responses.reviews;

namespace movielogger.api.models.responses.viewings;

public class ViewingResponse
{
    public int ViewingId { get; set; }
    public int UserId { get; set; }
    public bool Favourite { get; set; }
    public bool OwnsMovies { get; set; }
    public DateTime UpcomingViewDate { get; set; }
    public DateTime DateViewed { get; set; }
    public required MovieResponse Movie { get; set; }
    public ReviewResponse? Review { get; set; }
}
