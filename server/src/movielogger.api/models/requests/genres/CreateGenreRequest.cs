using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.genres;

public class CreateGenreRequest : IValidatable<CreateGenreRequest>
{
    public string Title { get; set; } = string.Empty;

    public IValidator<CreateGenreRequest> GetValidator()
        => new CreateGenreRequestValidator();
}