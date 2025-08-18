using FluentValidation;
using movielogger.api.models.requests.lists;

namespace movielogger.api.validation.validators;

public class AddMovieToListRequestValidator : AbstractValidator<AddMovieToListRequest>
{
    public AddMovieToListRequestValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("Movie ID must be greater than 0");
    }
}