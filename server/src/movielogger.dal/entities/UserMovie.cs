namespace movielogger.dal.entities;

public class UserMovie
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = default!;

    public bool Favourite { get; set; } = false;
    public bool OwnsMovie { get; set; } = false;
    public DateTime? UpcomingViewDate { get; set; }

    public ICollection<Viewing> Viewings { get; set; } = new List<Viewing>();
}
