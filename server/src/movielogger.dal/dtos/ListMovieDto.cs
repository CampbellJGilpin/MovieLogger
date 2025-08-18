namespace movielogger.dal.dtos;

public class ListMovieDto
{
    public int Id { get; set; }
    public int ListId { get; set; }
    public int MovieId { get; set; }
    public DateTime AddedDate { get; set; }
    public MovieDto? Movie { get; set; }
    public ListDto? List { get; set; }
}