using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.genres;

namespace movielogger.api.validators;

public class UpdateGenreRequestValidator : AbstractValidator<UpdateGenreRequest>
{
    public UpdateGenreRequestValidator()
    {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
    }
}