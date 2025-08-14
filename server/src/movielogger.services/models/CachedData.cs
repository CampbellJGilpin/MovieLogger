namespace movielogger.services.models;

public class CachedMoviesResult
{
    public IEnumerable<movielogger.dal.dtos.MovieDto> Movies { get; set; } = new List<movielogger.dal.dtos.MovieDto>();
    public int TotalCount { get; set; }
}

public class CachedUserMoviesResult
{
    public IEnumerable<movielogger.dal.dtos.UserMovieDto> Items { get; set; } = new List<movielogger.dal.dtos.UserMovieDto>();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}