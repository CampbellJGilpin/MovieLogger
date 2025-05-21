namespace movielogger.dal.dtos;

public class ViewingDto
{
    public int Id { get; set; }
    public int UserMovieId { get; set; }
    public DateTime DateViewed { get; set; }

    public ReviewDto? Review { get; set; }
}