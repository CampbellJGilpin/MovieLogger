namespace movielogger.dal.entities;

public class Genre
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}