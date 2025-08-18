using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.lists;

public class AddMovieToListRequest : IValidatable<AddMovieToListRequest>
{
    public int MovieId { get; set; }

    public IValidator<AddMovieToListRequest> GetValidator()
        => new AddMovieToListRequestValidator();
}