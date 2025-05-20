namespace movielogger.dal.entities;

public class Viewing
{
    public int Id { get; set; }

    public int UserMovieId { get; set; }
    public UserMovie UserMovie { get; set; } = default!;

    public DateTime DateViewed { get; set; }

    public Review? Review { get; set; }
}
