using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.genres;

public class UpdateGenreRequest : IValidatable<UpdateGenreRequest>
{
    public int GenreId { get; set; }
    public string Title { get; set; } = string.Empty;

    public IValidator<UpdateGenreRequest> GetValidator()
        => new UpdateGenreRequestValidator();
}