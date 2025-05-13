using FluentValidation;
using movielogger.api.models;

namespace movielogger.api.validators;

public class AddMovieRequestValidator : AbstractValidator<AddMovieRequest>
{
    public AddMovieRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.ReleaseDate).NotEmpty().WithMessage("ReleaseDate is required");
        RuleFor(x => x.GenreId).GreaterThan(0).WithMessage("Genre must be specified.");
    }
}