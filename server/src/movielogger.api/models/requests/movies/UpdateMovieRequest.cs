using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.movies;

public class UpdateMovieRequest : IValidatable<UpdateMovieRequest>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public int? GenreId { get; set; }

    public IValidator<UpdateMovieRequest> GetValidator()
        => new UpdateMovieRequestValidator();
}